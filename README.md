# HashJsonUtility
C# library
Target framework - .NET Standard 2.1 framework.

Library replaces JSON object properties and JSON string token (including JSON Path) values with their hashed representations (MD5, SHA256, etc.).

Reserved namespaces (including functions) are skipped. 
For example, 
{ 
	"XLS:SUM" : [
		1, 
		2
	] 
}

Valid JSON path segments are hashed separately. 
For example,
 - "$.['app:target'].['D_General'].['Q_Height']"
 replaced with
 - "$.['app:target'].['D9C2B564297DE5B6CB1A7CC645B8E9F0'].['0BFC75BB72CC455B95B45DAE2A88C319']"


Additionally, segments of JSON path separated by comma and placed in curly brackets are hashed separately and transformed into JSON path. 
For example,
 - "{app:target, D_General, Q_Height}"
replaced with
 - "$.['app:target'].['D9C2B564297DE5B6CB1A7CC645B8E9F0'].['0BFC75BB72CC455B95B45DAE2A88C319']"


Following reserved namespaces are supported:
 - "APP" 
 - "XLS" ( with one custom reserved function : "XLS:Compare". For example,  
		 {
			"XLS:COMPARE": {
				"left": "{app:target, D_General, Q_Height}", /* JSOP path must be hashed */
				"right": 170,
				"operator": ">="  /* value must remain original */
			}
		 }
	)