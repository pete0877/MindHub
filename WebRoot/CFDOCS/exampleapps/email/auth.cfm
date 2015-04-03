<CFINCLUDE TEMPLATE="_header.cfm">

<CFSET Session.Email = Form.Email>
<CFSET Session.Username = Form.Username>
<CFSET Session.Password = Form.Password>
<CFSET Session.POPserver = Form.POPserver>
<CFSET Session.SMTPserver = Form.SMTPserver>

<CFIF IsDefined("Form.SaveInfo")>
	<CFSET Expires = "NEVER">
<CFELSE>
	<CFSET Expires = "NOW">
</CFIF>

<CFCOOKIE NAME="SaveInfo" VALUE="TRUE" EXPIRES="#Expires#">

<CFLOOP LIST="Email,Username,POPserver,SMTPserver" INDEX="CurrItem">
	<CFCOOKIE NAME="#CurrItem#" VALUE="#evaluate('Form.' & CurrItem)#" EXPIRES="#Expires#">
</CFLOOP>

<FONT FACE="Helvetica" SIZE="-1"><B>Attempting to retrieve messages;<BR>
this may take a few moments.</B></FONT>

<SCRIPT LANGUAGE="JavaScript">
location.replace('refresh.cfm')
</SCRIPT>

<CFINCLUDE TEMPLATE="_footer.cfm">