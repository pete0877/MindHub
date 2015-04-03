
<!--- Test for localhost and abort to message window if other than "localhost" or "127.0.0.1" --->


<CFParam Name = "CGI.Host" default="">

<CFIf CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>

	<CFINCLUDE template="/cfdocs/sorry.htm">

	<CFABORT>

</CFIF>