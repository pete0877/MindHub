<CFINDEX Action="PURGE" Collection="cfdocumentation">

<CFINDEX Action="REFRESH" Type="PATH" 
			Collection="cfdocumentation" 
			Key="#ExpandPath('.')#" 
			URLPath="" 
			Extensions=".html,.htm" 
			Recurse="YES">

<HTML>
<BODY bgcolor="white" link="white" vlink="white" leftmargin=15 topmargin=15 background="bg30.gif" text="000000">
<br>

<font face="arial, geneva, helvetica" size="-1">
<p><b>Indexing Completed</b></p>

	<form action="searchmain.cfm" method="POST" target="main">
		<CFIF ParameterExists(SearchString)>
			<INPUT TYPE="hidden" NAME="SearchString" VALUE="<CFOUTPUT>#SearchString#</CFOUTPUT>">
		</CFIF>
		<INPUT TYPE="submit" VALUE="Return to Search">
	</FORM>
</font>

</BODY>
</HTML>
