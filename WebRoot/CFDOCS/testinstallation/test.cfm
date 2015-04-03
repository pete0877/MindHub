<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
<HEAD>

<TITLE>Verify Installation and Configuration</TITLE>

<META content="text/html; charset=windows-1252" http-equiv="Content-Type">
<META content="MSHTML 5.00.2314.1000" name="GENERATOR">

<SCRIPT language="Javascript">
<!--
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
//-->
</SCRIPT>

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
			<img src="../images/icons/back.gif" border=0></a><br>
		</td>

		<td>
			<a href="javascript: makeRemote();">
			<img src="../images/icons/search.gif" border=0></a><br>
		</td>

		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/home.gif" border=0></a><br>
		</td>
		
		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/uplevel.gif" border=0></a><br>
		</td>
		
		<td><A HREF="../Administering_ColdFusion_Server/contents.htm">
			<img src="../images/icons/next.gif" border=0></a><br>
		</td>
	</tr>
	</table><br><br><br><br>
<!-- End navigation table -->


<hr>

<A name="top"></A>

<h1>Verify Installation and Configuration</H1>


<P>If this test query returns a list of courses from the test database, ColdFusion is 
correctly installed and configured to work with your web server. </p>

<p>If an error occurs then consult the section below for hints on
diagnosing the problem. </p>

<p>

<FORM ACTION="courses.cfm" METHOD=post>

Department:  <SELECT Name="Department">
 <OPTION Value="BIOL" SELECTED>Biology
 <OPTION Value="CHEM">Chemistry
 <OPTION Value="ECON">Economics
 <OPTION Value="MATH">Math
</SELECT>    <INPUT Type="Submit" Value="Verify Query">

</FORM>

</p>


<H3>What problems might cause the test query to return an error?</H3>

<UL>
	<LI><a href="local.htm">You are browsing a local file (file://) rather than an 
	HTTP connection (http://)</a>.
	
	<LI><a href="odbc.htm">ODBC is not properly installed</a>.
	
	<LI><a href="apicgi.htm">The API/CGI-based execution is not working on 
	your server.</a>.
	
	<LI>ColdFusion did not start automatically after installation. Stop and 
	start the ColdFusion service.
</UL>



</BODY>

</HTML>
