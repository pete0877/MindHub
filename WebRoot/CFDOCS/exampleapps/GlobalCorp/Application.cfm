<!--- ColdFusion(r) Express Global Corp. Example Application --->





<!--- First, test for localhost and abort to message window if not --->



<CFParam Name = "CGI.Host" default="">

<cflock scope="server" timeout="60" type="ReadOnly">

	<cfset additionalInfo=Server.OS.AdditionalInformation>

</cflock>

<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>

	<CFINCLUDE template="../../sorry.htm">
	<CFABORT>

</CFIF>





<!--- 	The CFAPPLICATION tag creates an application framework for our applications. In this example, we have named the app cfexpress. We tell it turn on client management, and we have asked it to use cookies. --->

<CFAPPLICATION NAME="CFExpress" CLIENTMANAGEMENT="Yes" SETCLIENTCOOKIES="Yes">



<!--- 	By default, the user is not logged in --->

<CFPARAM NAME="Client.LoggedIn" DEFAULT="No">

<!--- 	This are default variables for preferences in discussion groups --->

<CFPARAM NAME="Client.PostOrder" DEFAULT="">

<CFPARAM NAME="Client.Sig" DEFAULT="">



<!--- 	We are now going to include a file that defines some site-wide variables. Notice how we check for application.initialized. globalvariables.cfm will set this value 	to true at the end of the file. This is cool because it means the variables will only be initialized once, which is all we need if you think about it. This is a little performance trick that will make the apps run a bit faster. --->

<CFIF NOT IsDefined("application.initialized") OR 1>

	<CFINCLUDE TEMPLATE="CommonFiles/global_variables.cfm">

</CFIF>



<!--- Are we logging into the application? --->

<CFIF IsDefined("Form.LoggingOn")>

	<!--- Check to see if valid --->

	<CFQUERY NAME="CheckUser" DATASOURCE="#application.ds#">

		SELECT	strEmpID

		FROM 	tblEmp

		WHERE 	strEmpID = '#Form.Employee#' 

		AND 	strPasswd = '#Form.Password#'

	</CFQUERY>

	<!--- Every query contains an element called "recordcount" which tells us how many rows are returned.  Iin this case we are checking to see if a user matched the validation query above. 	 	if the recordcount is 0 (no match in the database), the cfif statement 	below will be false, and the user will not be logged in.  Any number other than 0 is seen as true. 	 --->

	<CFIF CheckUser.RecordCount>

		<CFSET Client.LoggedIn="Yes">

		<CFSET Client.Employee_ID=CheckUser.strEmpID>

	</CFIF>

</CFIF>



<!--- 	This provides a mechanism for the user to logout. See the CommonFiles/_footer.cfm file for the link that triggers this.  --->

<CFIF IsDefined("Logout")>

	<CFSET Client.LoggedIn="No">

</CFIF>





<!--- 	Include the login form if the user is not logged in.  --->

<CFIF NOT Client.LoggedIn>

	<CFINCLUDE TEMPLATE="CommonFiles/logon_form.cfm">

	<CFABORT>

</CFIF>

