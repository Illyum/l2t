#IllyumL2T

##A parser to read delimiter-separated values in lines of text and map them into .NET CLR objects
One of the common tasks we as developers face is to read lines of text with values separated by some delimiter (comma, pipe, tab, etc.) while doing some basic manipulation like checking if it matches some pattern or convert it to a some of the .NET CLR datatype. IllyumL2T is an utility that eases such task.

###Installation
IllyumL2T consists of one single DLL: IllyumL2T.Core.dll. Although it might be added to a Visual Studio project like any other assembly, we recommend to use NuGet and the associated well-known procedure.

###How does it work?
First, create a class to represent every field expected to be read from the line of text. For example, suppose we have comma-separated values that intent to represent, say, an Order:
```
10248, 1.10, Address X, 10/10/2010
```
Let's assume that the fields are for order id (1), freight (1.10), ship address (Address X), and delivery date (10/10/2010). A possible .NET class to represent the Order could be:
```
class Order
{
  public short OrderId { get; set; }
  public decimal Freight { get; set; }
  public string ShipAddress { get; set; }
  public DateTime DeliveryDate { get; set; }
}
```
With those in place, the IllyumL2T `LineParser` class allows to read the field values and store them an Order instance like this:
```
var line = "10248, 1.10, Address X, 10/10/2010";
var lineParser = new LineParser<Order>();
var parseResult = lineParser.Parse(line, delimiter: ',');
var order = parseResult.Instance;
```
At this point, `order.OrderId` holds the integer value 10248, `order.Freight` holds the decimal value 1.10, `order.ShipAddress` holds the string value "Address X" and `order.DeliveryDate` holds the DateTime value October 10th 2010. Of course, any of the .NET CLR types can be used as needed and depending on the field values to be read.

An `InvalidOperationException` with message "Values mismatch fields definition" will be thrown if the number of values read doesn't match the number of properties.

###ParseResult
A `ParseResult` instance holds useful information about the just finished `LineParser` `Parse` method execution. In addition to the `Instance` property shown in the example above, the `Errors` property is a collection of the parse errors detected during the process. For example, consider the following code:
```
var line = "ABCD, 1.10, Address X,";
var lineParser = new LineParser<Order>();
var parseResult = lineParser.Parse(line, delimiter: ',');
```
Note that the order id field cannot be converted to the target int OrderId property and that the field for the delivery date is missing. The `parseResult.Errors` will contain two items:
```
foreach(var error in parseResult.Errors)
{
  Console.WriteLine(error);
}

// Will print...
//    OrderId: Unparsable System.Int16 >>> ABCD
//    DeliveryDate: Unparsable System.DateTime >>> 
```
The error message includes the target property that could not be set (OrderId and DeliveryDate in the previous example), its datatype, and the offending value (nothing appears when the value is null). Certainly, the Instance and Errors are the most useful properties of ParseResult.

###Null values
IllyumL2T parses missing values and treating them as nulls, not as errors. To accomplish this, just make the property a `Nullable<T>` one and that's it. For example, for the `DeliveryDate` property to accept a null value, the following declaration will do:
```
class Order
{
  public short OrderId { get; set; }
  public decimal Freight { get; set; }
  public string ShipAddress { get; set; }
  public DateTime? DeliveryDate { get; set; }
}

...

var line = "10248, 1.10, Address X, ";
var lineParser = new LineParser<Order>();
var parseResult = lineParser.Parse(line, delimiter: ',');
var order = parseResult.Instance;

Debug.Assert(order.DeliveryDate == null);
Debug.Assert(parseResult.Errors.Any() == false);
```
###ParseBehavior attribute
Sometimes values parsing might need special treatment. For example, thousands in decimal values might be separated by commas or periods while an email or URL value must match a pattern. IllyumL2T provides te ParseBehavior attribute to express some special treatment while parsing values. Take for example the following declaration:
```
class Person
{
  [ParseBehavior(Pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b")]
  public string Email { get; set; }
  
  [ParseBehavior(CultureName = "es-MX",
                 NumberStyle = NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint)]
  public decimal Salary { get; set; }
}

...

var line = "franl@illyum.com, $123.45";
var lineParser = new LineParser<Person>();
var parseResult = lineParser.Parse(line, delimiter: ',');
var person = parseResult.Instance;

Debug.Assert(person.Email == "franl@ilyum.com");
Debug.Assert(person.Salary == 123.45m);
```
The ParseBehavior Pattern is a regular expression pattern. CultureName, NumberStyle, and DateTimeStyle properties use the .NET support for Globalization and it is extensively documented in MSDN. Finally, the DateTimeFormat property serves for the exact same purpose documented in DateTime.ParseExact or DateTime.TryParseExact. Take in mind that this property works in conjunction with CultureName and DateTimeStyle for DateTime values parsing.

###Delimiter
IllyumL2T allows to configure the character to be used to separate field values through the second argument of the `LineParser` `Parse` method as shown in the examples above. But, what if the delimiter is part of the field being parsed? Take for example, the following line of text:
```
1, Say Hello, world! to the world!, 3.1416
```
If the comma is to be used as a field delimiter, the parsing will produce four values which may or may not what it is intended. But to prevent this, some programs like Microsoft(R) Excel encloses the field value in double quotes as shown below:
```
1, "Say Hello, world! to the world!", 3.1416
```
IllyumL2T supports parsing such fields as depicted in the following code:
```
class Foo
{
  public int IntegerProperty { get; set; }
  public string StringProperty { get; set; }
  public double DoubleProperty { get; set; }
}

...

var line = "1, \"Say Hello, world! to the world!\", 3.1416";
var lineParser = new LineParser<Foo>();
var parseResult = lineParser.Parse(line, delimiter: ',');
var foo = parseResult.Instance;

Debug.Assert(foo.IntegerProperty == 1);
Debug.Assert(foo.StringProperty == "Say Hello, world! to the world!");
Debug.Assert(foo.DoubleProperty == 3.1416);
```
Internally, IllyumL2T uses a regular expression that can be overriden through the SplitByDelimiterRegexPattern parameter in the application configuration file (web.config for web applications).

###FileParser
IllyumL2T provides the `FileParser` class when the lines are to be read from a text file. The following code shows an example of how to use it:
```
var filePath = "Orders.csv";
using(var reader = new StreamReader(filePath))
{
  var fileParser = new FileParser<Order>();
  var parseResults = fileParser.Read(reader, delimiter: ',', includeHeaders: false);
  
  ...
}
```
The `FileParser` `Read` method returns an `IEnumerable<ParseResult<T>>` (in this case, `IEnumerable<ParseResult<Order>`); every line and field successfully parsed (or the condition error if not) can be found out through the corresponding `ParseResult` as described earlier.

Of course, the `IEnumerable<ParseResult<T>>` returned by `FileParser` `Read` in combination with LINQ allows to do some fancy things like the following three examples:
####Example I
```
var filePath = "Orders.csv";
using(var reader = new StreamReader(filePath))
{
  var fileParser = new FileParser<Order>();
  
  // Retrieve the orders with delivery date in the month of July...
  var orders = fileParser.Read(reader, delimiter: ',', includeHeaders: false)
                         .Select(result => result.Instance)
                         .Where(order => order.DeliveryDate.Month = 7);
}
```
####Example II
```
var filePath = "Orders.csv";
using(var reader = new StreamReader(filePath))
{
  var fileParser = new FileParser<Order>();
  
  // Retrieve the orders descending sorted by id...
  var orders = fileParser.Read(reader, delimiter: ',', includeHeaders: false)
                         .Select(result => result.Instance)
                         .OrderByDescending(order => order.OrderId);
}
```
####Example III
```
var filePath = "Orders.csv";
using(var reader = new StreamReader(filePath))
{
  var fileParser = new FileParser<Order>();
  
  // Retrieve all the lines that could not be parsed successfully and the number of parsing errors...
  var lines = fileParser.Read(reader, delimiter: ',', includeHeaders: false)
                        .Where(parseResult => parseResult.Errors.Any())
                        .Select(parseResult => new { parseResult.Line, parseResult.Errors.Count() });
}
```
It is important to note that IllyumL2T is a utility for parsing text files, lines, and fields and it is not meant to do efficient queries, sorts, or any of the set-oriented processing that can be accomplished through other technologies and tools; the larger the file, the slower the performance. Reserve yourself some minutes to give IllyumL2T a try with your own files and let us know how it went.

Finally, take a look at the Unit Tests. It reveals more details on how to use IllyumL2T.
