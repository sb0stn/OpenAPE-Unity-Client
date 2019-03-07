# OpenAPE Unity (C#)

This project implements basic OpenAPE (https://openape.gpii.eu/) features for Unity in C#. 

The client implements the following methods:
* Login
* GetProfile (by id)
* UpdateProfile

There are some project specific things, such as the types of the used preference terms that need to be manually adapted to your specific use case for now.

Add the following classes and interfaces to your project:
* [Client.cs](Client.cs)
* [Personas.cs](Personas.cs)
* [PreferenceTerm.cs](PreferenceTerm.cs)
* [PreferenceTerms.cs](PreferenceTerms.cs)
* [ICoroutineExecutor.cs](ICoroutineExecutor.cs)

This project also depends on the following, which need to be added via nuget or manually.
* JSON.net

You will need to call the methods on the Client class from a MonoBehavior inside a coroutine:
```cs
private IEnumerator LoginUser(string username, string password, string contextId)
{
	yield return OpenApeClient.Login(username, password, (status, result) =>
	{
		if (status)
		{
			StartCoroutine(GetUserPreferences (contextId));
		}
	});
}
```

Your caller needs to implement the ```ICoroutineExecutor``` interface and therefore must be a MonoBehavior:
```cs
public void StartChildCoroutine(IEnumerator coroutineMethod)
{
	StartCoroutine(coroutineMethod);
}
```
