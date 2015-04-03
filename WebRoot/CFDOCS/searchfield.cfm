<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Untitled</TITLE>
</HEAD>

<BODY bgcolor="#669966" leftmargin=1>
<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="ccFFcc">
<tr>
	<td width=100%>
		<table>
		<tr>
			<td align="right" width=135>
			<FONT FACE="Arial, Helvetica" SIZE=-1><b>
				Enter&nbsp;Search&nbsp;Criteria&nbsp;&nbsp;
			</b></font>
	</td><td>
	<form action="searchmain.cfm?RequestTimeout=300" method="POST" target="main" name="Search">
	
<CFOUTPUT>
	<INPUT TYPE=text NAME=SearchString SIZE=20
<CFIF ParameterExists(SearchString)>VALUE="#SearchString#"</CFIF>>
	<INPUT TYPE=SUBMIT NAME=search1 VALUE="Search"><br>
</CFOUTPUT>
</td></tr></table>
	</FORM>
	<br>
	<br>
	<br>

</td>
	<td bgcolor="339933" width="16" nowrap>&nbsp;</td>
</tr>
</table>

</BODY>
</HTML>
