<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<CFSET RootPath = Reverse(Replace(Reverse(GetTemplatePath()),Reverse("admin#Sep##GetFileFromPath(GetTemplatePath())#"),""))>

<CFFILE ACTION="write" FILE="#rootpath#binarydata/ray.txt" OUTPUT="rrr">

<CFDIRECTORY ACTION="list" DIRECTORY="#rootpath#binarydata" NAME="test">
<CFOUTPUT query="test">#name#<BR></CFOUTPUT>
<CFFILE ACTION="UPLOAD"
		FILEFIELD="UploadedFile"
		DESTINATION="#RootPath#binarydata"
		NAMECONFLICT="SKIP">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Untitled</TITLE>
</HEAD>

<BODY>

<CFIF File.FileExisted>

	<CFOUTPUT>
		<SCRIPT LANGUAGE="JAVASCRIPT">
		   alert('The file "#File.ClientFile#"\nalready exists on the server.\n\nPlease rename your file and try again.');
		   location.replace('modifycontent.cfm?ContentIDList=#Replace(ContentIDList,"_","-","ALL")#&C#ContentIDList#=#URLEncodedFormat("|0")#');
		</SCRIPT>
		The file #File.AttemptedServerFile# already exists on the server.<BR>
		Please rename your file and try again.
	</CFOUTPUT>

<CFELSE>

	<CFLOCATION URL="modifycontent.cfm?ContentIDList=#Replace(ContentIDList,"_","-","ALL")#&C#ContentIDList#=#File.ServerFile#" ADDTOKEN="NO">

</CFIF>	

</BODY>
</HTML>
