<CFPARAM NAME="Location" DEFAULT="1">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Add Instance</TITLE>

<SCRIPT LANGUAGE="JAVASCRIPT">

function showStartLayer() {
	if (document.all != null)
		StartDateLayer.style.visibility='visible';
}

function hideStartLayer() {
	if (document.all != null)
		document.all.StartDateLayer.style.visibility='hidden';
}

function showEndLayer() {
	if (document.all != null)
		document.all.EndDateLayer.style.visibility='visible';
}

function hideEndLayer() {
	if (document.all != null)
		document.all.EndDateLayer.style.visibility='hidden';
}

</SCRIPT>

<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>

</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A><BR>
		<CFOUTPUT><A HREF="properties.cfm?ObjectID=#ObjectID#">Return to Object #ObjectID#</A></CFOUTPUT>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/createinstance.gif" WIDTH=131 HEIGHT=20 BORDER=0 ALT="Create Instance"></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><CFOUTPUT>Creating an instance for <B>Object #ObjectID#</B> on <B>#TemplatePath#</B>, in <B>Location #Location#</B>...</CFOUTPUT></P>

<BR>

<FORM NAME="SchedulingForm" ACTION="createinstance4.cfm" METHOD="POST">

<CFOUTPUT>
	<INPUT TYPE="HIDDEN" NAME="ObjectID" VALUE="#ObjectID#">
	<INPUT TYPE="HIDDEN" NAME="PageID" VALUE="#PageID#">
	<INPUT TYPE="HIDDEN" NAME="TemplatePath" VALUE="#TemplatePath#">
	<INPUT TYPE="HIDDEN" NAME="Location" VALUE="#Location#">
</CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Set scheduling information:</B></FONT></P>

<TABLE BORDER="0" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD>

		<TABLE BORDER="2" CELLPADDING="5" CELLSPACING="0">
		<TR>
			<TD>

			<P><FONT FACE="Helvetica"><B>Start Time</B></FONT></P>

			<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<INPUT TYPE="RADIO" NAME="StartType" VALUE="0" onClick="hideStartLayer()" CHECKED>Start Immediately<BR><BR>
			<INPUT TYPE="RADIO" NAME="StartType" VALUE="1" onClick="showStartLayer()">Start at Specified Time
			</FONT></P>

			<DIV ID="StartDateLayer" STYLE="visibility: hidden">
				<BLOCKQUOTE>
					<CF_dateselect FORM="SchedulingForm" SELECTNAME="Start" STARTYEAR="1998" ENDYEAR="2012" SHOWTIME=TRUE>
				</BLOCKQUOTE>
			</DIV>

			</TD>

		</TR>
		</TABLE>

	</TD>

	<TD>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</TD>

	<TD>		

		<TABLE BORDER="2" CELLPADDING="5" CELLSPACING="0">
		<TR>
			<TD>

			<P><FONT FACE="Helvetica"><B>End Time</B></FONT></P>

			<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<INPUT TYPE="RADIO" NAME="EndType" VALUE="0" onClick="hideEndLayer()" CHECKED>Never Expire<BR><BR>
			<INPUT TYPE="RADIO" NAME="EndType" VALUE="1" onClick="showEndLayer()">End at Specified Time
			</FONT></P>

			<DIV ID="EndDateLayer" STYLE="visibility: hidden">
				<BLOCKQUOTE>
					<CF_DateSelect FORM="SchedulingForm" SELECTNAME="End" STARTYEAR="1998" ENDYEAR="2012" SHOWTIME=TRUE>
				</BLOCKQUOTE>
			</DIV>

			</TD>
		</TR>
		</TABLE>

	</TD>
</TR>
</TABLE>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>
