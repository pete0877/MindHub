<!--- If not localhost, abort to message window --->

<CFParam Name = "CGI.Host" default="">

<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>


	<CFINCLUDE template="/cfdocs/sorry.htm">

	<CFABORT>

</CFIF>


<!--- Hard coded UUIDs refer for docs --->

<CFSET DOC1 = "BA1EA6CC-7D79-11D3-A928005004218998">
<CFSET DOC2 = "BA1EA6CD-7D79-11D3-A928005004218998">
<CFSET DOC3 = "BA1EA6CE-7D79-11D3-A928005004218998">
<CFSET DOC4 = "BA1EA6CF-7D79-11D3-A928005004218998">
<CFSET DOC5 = "BA1EA6D0-7D79-11D3-A928005004218998">
<CFSET DOC6 = "BA1EA6D1-7D79-11D3-A928005004218998">
<CFSET DOC7 = "BA1EA6D2-7D79-11D3-A928005004218998">
<CFSET DOC8 = "BA1EA6D3-7D79-11D3-A928005004218998">

<CFIF IsDefined("URL.DocID") AND Len(URL.DocID) LT 3 AND Val(URL.DocID)>
	<CFTRY>
		<CFSET URL.DocID = Evaluate("DOC#URL.DocID#")>
		<CFCATCH>
			<CFSET URL.DocID = DOC1>
		</CFCATCH>
	</CFTRY>

</CFIF>