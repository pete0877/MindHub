<CFIF Server.OS.Name IS NOT "UNIX">
	<!-- Lookup department name -->
	<CFQUERY Name="Department" DataSource="cfsnippets">
		SELECT Dept_Name FROM Departments
			WHERE Dept_ID = '#Form.Department#' 
	</CFQUERY>

	<!-- Get courses offered by department -->
	<CFQUERY Name="Courses" DataSource="cfsnippets">
		SELECT * FROM CourseList
			WHERE Dept_ID = '#Form.Department#' 
	</CFQUERY>
<CFELSE>
	<!-- Lookup department name -->
	<CFQUERY Name="Department" DataSource="cfsnippets">
		SELECT Dept_Name FROM Departments
			WHERE Dept_ID = '#Form.Department#' 
	</CFQUERY>

	<!-- Get courses offered by department -->
	<CFQUERY Name="Courses" DataSource="cfsnippets">
		SELECT * FROM CourseList
			WHERE Dept_ID = '#Form.Department#' 
	</CFQUERY>
</CFIF>

<HTML>
<HEAD><TITLE>ColdFusion University Course Search</TITLE>


<script language="Javascript">

var DocRemote = 0;

function makeRemote(){

	if(DocRemote){
		if(DocRemote.closed){
			DocRemote = 0;
			makeRemote();
		}else{
			DocRemote.focus();
		}
	}else{
		DocRemote = window.open('../search.cfm', 
					    'Search', 
					    'scrollbars,resizable,width=510,height=300');
    	}
}

</script>


<script language="JavaScript1.1">
<!--
	blank = new Image();
	blank.src = 'images/mouseovers/blank.gif';
	
	imgBack = new Image();
	imgBack.src = 'images/mouseovers/back.gif';

	imgSearch = new Image();
	imgSearch.src = 'images/mouseovers/search.gif';

	imgTop = new Image();
	imgTop.src = 'images/mouseovers/toplevel.gif';

	imgUp = new Image();
	imgUp.src = 'images/mouseovers/uplevel.gif';

	imgNext = new Image();
	imgNext.src = 'images/mouseovers/next.gif';
//-->
</script>




<script language="JavaScript">
<!-- Hide JavaScript

   if (navigator.appName == "Netscape"){
      document.write('<LINK REL=STYLESHEET HREF="../allaire_ns.css" TYPE="text/css">');
      }
   else{
      document.write('<LINK REL=STYLESHEET HREF="../allaire_ie.css" TYPE="text/css">');
   }
//-->
</script>



</head>

<body>


<!-- Start navigation table -->

	<table border="0" cellspacing="0" cellpadding="5" align="right">
	<tr>
		<td>
			<img src="../images/mouseovers/blank.gif" name="DescText" border=0><br>
		</td>

		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/back.gif" alt="ColdFusion documentation" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgBack.src"></a><br>
		</td>

		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/uplevel.gif" alt="ColdFusion documentation" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgUp.src"></a><br>
		</td>

		<td>
			<A HREF="../examplehome.cfm">
			<img src="../images/icons/next.gif" alt="Example applications" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgNext.src"></a><br>
		</td>

		<td>
			<a href="../index.htm">
			<img src="../images/icons/home.gif" alt="ColdFusion Welcome page" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgTop.src"></a><br>
		</td>
		
		<td>
			<a href="javascript: makeRemote();">
			<img src="../images/icons/search.gif" alt="Search the docs" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgSearch.src"></a><br>
		</td>

	</tr>
	</table><br><br><br><br>

<!-- End navigation table -->

<hr>




<a name="top"></A>
<h1>Verify Installation and Configuration</H1>




<H2>Test Query Results</h2>


<CFIF Server.OS.Name IS NOT "UNIX">
  <CFOUTPUT Query="Department">
	<H3>Courses Offered by the #Dept_Name# Department</H3>
  </CFOUTPUT>

  <!-- Table displaying courses offered -->
  <CFTABLE Query="Courses" HTMLTABLE BORDER COLHEADERS MAXROWS=200 >
	<CFCOL Header="<B>Number</B>" Width=10 Text="#CorNumber#">
	<CFCOL Header="<B>Course Name</B>" Width=25 Text="#CorName#">
	<CFCOL Header="<B>Level</B>" Width=15 Align=Right Text="#CorLevel#">
  </CFTABLE>

<CFELSE>

  <CFOUTPUT Query="Department">
	<H3>Courses Offered by the #Dept_Name# Department</H3>
  </CFOUTPUT>

  <!-- Table displaying courses offered -->
  <CFTABLE Query="Courses" HTMLTABLE BORDER COLHEADERS MAXROWS=200>
	<CFCOL Header="<B>Number</B>" Width=10 Text="#Trim(CorNumber)#">
	<CFCOL Header="<B>Course Name</B>" Width=25 Text="#Trim(CorName)#">
	<CFCOL Header="<B>Level</B>" Width=15 Align=Right Text="#Trim(CorLevel)#">
  </CFTABLE>
</CFIF>
<BR>

<P>
ColdFusion has been correctly installed and configured to work 
with your Web server.</p>

<P>
Now that everything has been set up properly, you probably want
to learn about all of the things you can do with ColdFusion. The 
<a href="../examplehome.cfm">Example Applications</a>
page has pointers to several resources to help you get started.</p>



</BODY>
</HTML>
