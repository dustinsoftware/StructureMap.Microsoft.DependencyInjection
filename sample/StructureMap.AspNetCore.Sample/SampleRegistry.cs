using StructureMap;

public class SampleRegistry : Registry
{
    public SampleRegistry()
    {
        // Add your StructureMap configuration here
        For<ISampleService>().Singleton().Use<SampleService>();
    }
}

public class SampleService : ISampleService
{
    public string GetMessage()
    {
        return "Hello from SampleService!";
    }
}

public interface ISampleService
{
    string GetMessage();
}
