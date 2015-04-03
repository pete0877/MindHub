<cfset templatePath=getBaseTemplatePath()>
 
<!--- [sg 11/18/99] Test for NT path first and create docDir variable --->

<cfif FindNoCase("cfdocs\testinstallation\test.cfm", templatepath)>

	<cfset docDir = ReplaceNoCase(templatePath, "cfdocs\testinstallation\test.cfm", "cfdocs\CFML_Language_Reference\contents.htm")>


	<cfelse>

	<!--- [sg 11/18/99] If not an NT path, must be UNIX --->		
	<cfset docDir = Replace(templatePath, "cfdocs/testinstallation/test.cfm", "cfdocs/CFML_Language_Reference/contents.htm")>
							
</cfif>

<!--- [sg 11/18/99] If docs are installed, proceed, otherwise, stick up the sorry.htm messagebox --->			
<!--- <cfif NOT DirectoryExists(docDir)> --->

<cfif NOT FileExists(docDir)>


		
	<cfinclude template="/CFIDE/administrator/docs/sorry.htm">
		
	<cfabort>
		
</cfif>

<!--- Then, test for localhost and abort to message window if other than "localhost" or "127.0.0.1" --->


<CFParam Name = "CGI.Host" default="">

<CFIf CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>

	<CFINCLUDE template="/CFIDE/administrator/docs/sorry.htm">

	<CFABORT>

</cfif>