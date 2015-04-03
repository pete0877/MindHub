<CFQUERY DATASOURCE="CFX" NAME="GetDoc">
	SELECT * FROM tblDocEx
	WHERE strDocID = '#DocID#'
</CFQUERY>

<CFSET NewBody = Replace(GetDoc.glbBody, '{CODE}', '<FONT FACE="Courier New" SIZE="3" COLOR="##FF0000">', 'ALL')>
<CFSET NewBody = Replace(NewBody, '{/CODE}', '</FONT>', 'ALL')>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Untitled</TITLE>
<BASE TARGET="_parent">
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#FFFFD9">

<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">

<CFOUTPUT>#NewBody#</CFOUTPUT>

</FONT>

</BODY>
</HTML>
