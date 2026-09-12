using Xunit;
using ProformaViewer.Services;
namespace ProformaViewer.Tests;
public class PageSpecParserTests {
 [Theory][InlineData("15",new[]{15})][InlineData("16x4",new[]{16,16,16,16})][InlineData("16,17",new[]{16,17})][InlineData("1,2",new[]{1,2})]
 public void ParsesDatabaseFormats(string value,int[] expected)=>Assert.Equal(expected,PageSpecParser.Parse(value));
 [Theory][InlineData("")][InlineData("0")][InlineData("abc")][InlineData("2x0")] public void RejectsInvalidFormats(string value)=>Assert.Throws<FormatException>(()=>PageSpecParser.Parse(value));
}
