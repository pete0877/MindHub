<!--- First, test for locahost and abort to message window if not --->

<CFParam Name = "CGI.Host" default="">

<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>


	<CFINCLUDE template="/cfdocs/sorry.htm">

	<CFABORT>

</CFIF>


<!--- application file, exampleapp\email --->
<!--- this file restricts the use of the email directory to the 127.0.0.1 or localhost address --->

<CFAPPLICATION NAME="crazycab" SESSIONMANAGEMENT="Yes" SETCLIENTCOOKIES="Yes" SESSIONTIMEOUT="#CreateTimeSpan(0,1,30,0)#">

<!--- specify location, interval, and switch for redirection (defaults to 1)--->
<!--- if redirection is desired, set redto 1 --->
<CFPARAM name="variables.new_location" default="http://www.allaire.com/documents/">
<CFPARAM name="variables.new_interval" default="10">
<CFPARAM name="variables.redirect_switch" default="0">

<!--- restrict IP here --->
<CFIF cgi.REMOTE_ADDR NEQ "127.0.0.1" AND 0>


<CFINCLUDE template="_header.cfm">

<CFIF redirect_switch eq 1>
	<CFOUTPUT>
		<CFHTMLHEAD text="<META Http-equiv='Refresh' Content='#new_interval#; url=#new_location#'>">		
	</CFOUTPUT>
</CFIF>


<h3>
<P>Please view your copy of ColdFusion locally<BR>
	 to use the CrazyCab Application.

<CFIF redirect_switch eq 1>
	<CFOUTPUT>
	<P>Be patient; you will be redirected to <BR>
	#new_location# in #new_interval# seconds.
	</CFOUTPUT>
</CFIF>
</h3>

<CFINCLUDE template="_footer.cfm">


<CFABORT>
</CFIF>


<!--- Make sure we're logged in correctly --->
<CFIF GetFileFromPath(GetTemplatePath()) NEQ "login.cfm" AND GetFileFromPath(GetTemplatePath()) NEQ "auth.cfm">
	<CFIF NOT IsDefined("Session.POPserver")>
		<CFLOCATION URL="login.cfm" ADDTOKEN="NO">
	<CFELSEIF Session.POPserver IS "">
		<CFLOCATION URL="login.cfm" ADDTOKEN="NO">
	</CFIF>
</CFIF>
