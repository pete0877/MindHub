<CFSET NewBody = Form.MessageBody>

<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<!--- Create main temp directory if it doesn't already exist --->
<CFSET TempDir = GetTempDirectory() & "CrazyCab#Sep#">
<CFTRY>
	<CFDIRECTORY ACTION="CREATE" DIRECTORY="#TempDir#">
	<CFCATCH TYPE="ANY">
	</CFCATCH>
</CFTRY>

<!--- Create outbound temp directory if it doesn't already exist --->
<CFSET AttachDir = TempDir & "outbound#Sep#">
<CFTRY>
	<CFDIRECTORY ACTION="CREATE" DIRECTORY="#AttachDir#">
	<CFCATCH TYPE="ANY">
	</CFCATCH>
</CFTRY>

<!--- Word-wrap the text that will be sent --->
<CF_Wrap VARIABLE="NewBody" WIDTH="80">

<!--- If this message has a file attachment... --->
<CFIF Form.AttachFile NEQ "">

	<!--- ...save the file to the server... --->
	<CFFILE ACTION="UPLOAD"
		FILEFIELD="AttachFile"
		DESTINATION="#AttachDir#"
		NAMECONFLICT="OVERWRITE">

	<!--- ...and send the message with file attached... --->
	<CFMAIL FROM="#Session.Email#"
		TO="#Form.To#"
		CC="#Form.cc#"
		SUBJECT="#Form.Subject#"
		SERVER="#Session.SMTPserver#"
		MIMEATTACH="#AttachDir##File.ServerFile#">#NewBody#</CFMAIL>

<!--- ...otherwise... --->
<CFELSE>

	<!--- ...just send the text with no attachment. --->
	<CFMAIL FROM="#Session.Email#"
		TO="#Form.To#"
		CC="#Form.cc#"
		SUBJECT="#Form.Subject#"
		SERVER="#Session.SMTPserver#">#NewBody#</CFMAIL>

</CFIF>

<CFLOCATION URL="messagelist.cfm" ADDTOKEN="NO">