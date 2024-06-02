using ServerSide.infrastructure;

namespace TestReport
{
    public class UnitTest1
    {
        [Fact]
        public void TestJson()
        {
            JsonToDataTable jsonToDataTable = new JsonToDataTable();
            jsonToDataTable.TestJson(); 
        }
    }
}