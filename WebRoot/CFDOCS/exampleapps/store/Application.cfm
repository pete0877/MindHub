<!--- Test for localhost, and abort to message window if not --->


<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>


	<frameset rows="100%, *" onLoad=window.resizeTo(550,300)>
		<frame frameborder="yes" 
			src="/cfdocs/sorry.htm" 
			name="sorry">
		</frame>
	</frameset>	 

	<CFABORT>

</CFIF>


<!--

Online Store 1.0
----------------

This is an example app for Allaire Cold Fusion 4.0.  Find out
more about the Cold Fusion Web Application Development System at
Allaire's web site: http://www.allaire.com.

-->

<CFAPPLICATION NAME="CF40_STORE" SESSIONMANAGEMENT="Yes" SESSIONTIMEOUT="#CreateTimeSpan(0,1,30,0)#">

<!--- Create shopping cart variables if they don't already exist --->
<!--- <CFPARAM NAME="Session.StoreItems" DEFAULT=""> 
<CFPARAM NAME="Session.StoreQuantities" DEFAULT=""> --->
<CFPARAM name="Application.datasource" default="cfx">
<CFPARAM name="Application.TaxRate" default="0.0725">
<!--- database type can be "SQL" or "Access" --->
<CFPARAM name="Application.dbtype" default="Access">

