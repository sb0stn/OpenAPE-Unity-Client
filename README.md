# OpenApe-C-Sharp

Just add the following classes and interfaces to your project:
* [Client.cs](Client.cs)
* [Persona.cs](Persona.cs)
* [PreferenceTerm.cs](PreferenceTerm.cs)
* [PreferenceTerms.cs](PreferenceTerms.cs)
* [ICoroutineExecutor.cs](ICoroutineExecutor.cs)

This project also depends on the following, which need to be added via nuget.
* JSON.net

You will need to call the methods on the Client class from a MonoBehavior inside a coroutine:
```cs
private IEnumerator LoginUser(string username, string password)
{
	yield return OpenApeClient.Login(username, password, (status, result) =>
	{
		if (status)
		{
			StartCoroutine(GetUserPreferences (Persona.Olaf.Id));
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

# OpenAPE Doku
## Nutzer
### Olaf
#### Anmeldedaten
* **User:** sh18_olaf
* **Passwort:** sh18demoolaf
* **E-Mail:** sh18_olaf@0py.de

#### UserContext und Preferences
**Context ID:** 5ad9f7a438f55331474ccdf4

| Preference Term | Wert | Bedeutung |
|---|---|---|
| language | de-DE | Hauptsprache des Interfaces ist auf deutsch. |
| auditoryOutLanguage | de-DE | Audioausgabe ist deutsch. |
| highContrastEnabled | true | Hoher Kontrast des UI ist aktiviert. |
| voiceControlEnabled | true | Steuerung per Stimme ist aktiviert. |
| fontSize | 16 | Schrift soll 16 Punkt sein. | 
| speechRate | 120 | Sprachausgabe erfolgt mit 120 Wörtern/Minute. |
| pitch | 0.5 | Sprachausgabe erfolgt mit mittlerem (default) Pitch. |
| volumeTTS | 0.75 | Sprachausgabe erfolgt mit 75% der maximalen Lautstärke. |


### Mary
#### Anmeldedaten
* **User:** sh18_mary
* **Passwort:** sh18demomary
* **E-Mail:** sh18_mary@0py.de

#### UserContext und Preferences
**Context ID:** 5ad9f74c38f55331474ccdf0

| Preference Term | Wert | Bedeutung |
|---|---|---|
| language | de-DE | Hauptsprache des Interfaces ist auf deutsch. |

### Hannes
#### Anmeldedaten
* **User:** sh18_hannes
* **Passwort:** sh18demohannes
* **E-Mail:** sh18_hannes@0py.de

#### UserContext und Preferences
**Context ID:** 5ad9f77138f55331474ccdf2

| Preference Term | Wert | Bedeutung |
|---|---|---|
| language | de-DE | Hauptsprache des Interfaces ist auf deutsch. |



## Weitere Infos
* Bitte beachtet, dass die verwendeten E-Mail Adressen nicht aktiv sind, bei Bedarf aber geändert bzw. aktiviert werden können.
* Die Preference Terms sind - bis jetzt zumindest - alle relativ zu http://registry.gpii.eu/common/, also z.B. http://registry.gpii.eu/common/language.
* Nicht alle Preference Terms müssen zwangsläufig in unserer Anwendung beachtet werden.
* Es könnten auch weitere hinzugefügt werden bei Bedarf, durch anwendungsspezische Terms.


## Wichtige APIs
*Die API URLs und kein Slash am Ende...*

### Login
#### Request
<pre>
curl \
	--request POST \
	--url https://openape.gpii.eu/token \
	--header 'content-type: application/x-www-form-urlencoded' \
	--data 'grant_type=password&username=<strong>user</strong>&password=<strong>password</strong>'
</pre>

#### Response
<pre>
{
	"access_token": "<strong>token</strong>",
	"expires_in": "1440"
}
</pre> 

### GetUserContext
#### Request
<pre>
curl \
	--request GET \
	--url https://openape.gpii.eu/api/user-contexts/<strong>contextId</strong> \
	--header 'authorization: <strong>access_token</strong>' \
	--header 'content-type: application/json'
</pre>

#### Response
<pre>
{
	"default": {
		"name": "<strong>Name</strong>'s preferences (SmartHome SS18)",
		"preferences": {
			"http://registry.gpii.eu/common/language": "de-DE",
			<strong>/* ... */</strong>
		}
	}
}
</pre>

### UpdateUserContext
#### Request
<pre>
curl --request PUT \
  --url http://openape.gpii.eu/api/user-contexts/<strong>contextId</strong> \
  --header 'authorization: <strong>access_token</strong>' \
  --header 'content-type: application/json' \
  --data '<strong>updatedProfile</strong>'
</pre>

#### Response
*No body in response*
