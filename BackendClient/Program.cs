using BackendClient;

// IndexHub
Console.WriteLine("IndexHub...");
try
{
    await IndexHub.Run();
}
catch (Exception e)
{ 
    Console.Error.WriteLine(e);
}
Console.WriteLine("...IndexHub");
Console.WriteLine("");
Console.WriteLine("AuthorizeHub...");
// AuthorizeHub
try
{
    await AuthorizeHub.Run();
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}
Console.WriteLine("...AuthorizeHub");

Console.ReadKey();