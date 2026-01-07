using System.Collections;

namespace WebApi.Test.InlineData;

public class CultureInlineDataTest : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["pt"];
        yield return ["en"];
        yield return ["ja"];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}