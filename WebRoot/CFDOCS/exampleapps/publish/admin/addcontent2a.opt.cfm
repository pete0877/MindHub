<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<CFSET RootPath = Reverse(Replace(Reverse(GetTemplatePath()),Reverse("admin#Sep##GetFileFromPath(GetTemplatePath())#"),""))>

<CFFILE ACTION="UPLOAD"
		FILEFIELD="UploadedFile"
		DESTINATION="#RootPath#binarydata"
		NAMECONFLICT="SKIP">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE></TITLE>
</HEAD>

<BODY>

<CFIF File.FileExisted>

	<CFOUTPUT>
		<SCRIPT LANGUAGE="JAVASCRIPT">
		   alert('The file "#File.ClientFile#"\nalready exists on the server.\n\nPlease rename your file and try again.');
		   location.replace('addcontent2.opt.cfm?CurrItem=#CurrItem#&CurrPos=#CurrPos#&Type#Replace(CurrItem,"-","_","ALL")#=#URLEncodedFormat("|0")#');
		</SCRIPT>
		The file #File.AttemptedServerFile# already exists on the server.<BR>
		Please rename your file and try again.
	</CFOUTPUT>

<CFELSE>

	<CFLOCATION URL="addcontent2.opt.cfm?CurrItem=#CurrItem#&CurrPos=#CurrPos#&Type#Replace(CurrItem,"-","_","ALL")#=#File.ServerFile#" ADDTOKEN="NO">

</CFIF>	

</BODY>
</HTML>
