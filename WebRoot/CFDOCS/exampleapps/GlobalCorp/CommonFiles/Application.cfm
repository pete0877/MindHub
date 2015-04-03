

<!--- Test for localhost and abort to message window if other than "localhost" or "127.0.0.1" --->

<CFAPPLICATION NAME="CFExpress" CLIENTMANAGEMENT="Yes" SETCLIENTCOOKIES="Yes">



<cflock scope="server" timeout="60" type="ReadOnly">

	<cfset additionalInfo=Server.OS.AdditionalInformation>

</cflock>



<CFParam Name = "CGI.Host" default="">



<CFIf CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>





	<CFINCLUDE template="../../../sorry.htm">



	<CFABORT>






</CFIF>