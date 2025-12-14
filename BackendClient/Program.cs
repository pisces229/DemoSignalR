using BackendClient;

try
{
    await Task.Delay(3000);
    Task.WaitAll([
        //BackplaneHub.Run("http://localhost:5231"),
        //BackplaneHub.Run("http://localhost:5232"),
        BackplaneHub.Run("http://localhost:8080"),
        //AuthorizeHub.Run(),
    ]);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.ToString());
}
Console.ReadLine();