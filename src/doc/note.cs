/*


[h1] Überschrift [/h1]
[b] Fett [/b]
[u] Unterstrichen [/u]
[i] Kursiv [/i]
[strike] Durchgestrichener Text [/strike]
[url=store.steampowered.com] Link [/url]

[list]
    [*]Liste mit Aufzählungspunkten
    [*]Liste mit Aufzählungspunkten
    [*]Liste mit Aufzählungspunkten
[/list]




Überschrift
-----------

[h1] Überschrift [/h1]


[**Link**](https://)

[url=https://] Link [/url]


* Liste mit Aufzählungspunkten

[list]
[*]Liste mit Aufzählungspunkten
[/list]


*/

void x(List<string> line)
{
	for (int i = 1; i < line.Count; i++)
	{
		if (line[i].StartsWith("-----------"))
		{
			line[i-1] = "[h1]"+ line[i-1] +"[/h1]";
			line.RemoveAt(i);
			i--;
		}
		// \[\*\*(.*?)\*\*\]\((.*?)\)
		Regex rx = new Regex(@"\[\*\*(.*?)\*\*\]\((.*?)\)", RegexOptions.Compiled);
	}
}
























