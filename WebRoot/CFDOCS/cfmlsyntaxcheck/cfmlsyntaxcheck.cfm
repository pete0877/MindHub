<!--- Restrict to local server --->

<CFSET isAllowedToExecute = (CGI.REMOTE_ADDR IS "127.0.0.1")>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
<HEAD>

<TITLE>CFML Syntax Checker</TITLE>

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
		DocRemote = window.open('search.cfm', 
					    'Search', 
					    'scrollbars,resizable,width=510,height=300');
    	}
}
//-->
</SCRIPT>

<script language="JavaScript1.1">
<!--
	blank = new Image();
	blank.src = '../images/mouseovers/blank.gif';
	
	imgBack = new Image();
	imgBack.src = '../images/mouseovers/back.gif';

	imgSearch = new Image();
	imgSearch.src = '../images/mouseovers/search.gif';

	imgTop = new Image();
	imgTop.src = '../images/mouseovers/toplevel.gif';

	imgUp = new Image();
	imgUp.src = '../images/mouseovers/uplevel.gif';

	imgNext = new Image();
	imgNext.src = '../images/mouseovers/next.gif';
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
			<img src="../images/icons/back.gif" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgBack.src"></a><br>
		</td>

		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/uplevel.gif" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgUp.src"></a><br>
		</td>
		
		<td><A HREF="../Administering_ColdFusion_Server/contents.htm">
			<img src="../images/icons/next.gif" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgNext.src"></a><br>
		</td>

		<td>
			<a href="../dochome.htm">
			<img src="../images/icons/home.gif" border=0 OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgTop.src"></a><br>
		</td>

		<td>
			<a href="javascript: makeRemote();" OnMouseOut="DescText.src=blank.src"
			OnMouseOver="DescText.src=imgSearch.src">
			<img src="../images/icons/search.gif" border=0></a><br>
		</td>
		
	</tr>

	</table><br><br><br><br>
<!-- End navigation table -->


<hr>

<A name="top"></A>
<h1>CFML 4.5 Syntax Checker</h1>



<p>The ColdFusion Server 4.0 release introduced stricter enforcement of CFML syntax rules. 
The CFML Syntax Checker included in ColdFusion 4.5 can help you isolate syntax errors that versions of ColdFusion following the 4.0 release now enforce. Strict checking can uncover hidden bugs and other types of undesirable behaviors in your CFML templates. Allaire recommends that you always use the strictest possible level of CFML template validation.</p>

<p>In rare cases, the more relaxed validation mechanisms used by
previous versions of ColdFusion may have allowed you to use
syntactically incorrect CFML constructs. The CFML Syntax Checker
is a simple application that can aid you in the process of
discovering non-conforming CFML syntax. </p>

<p>Use the form below to specify the directory where your CFML
pages are located. If you have some legacy DBML code, you may
want to change the file search filter to <em>*.?fm</em> so that
both <em>*.dbm</em> and <em>*.cfm</em> files are checked. If you
do not want to look for CFML templates in nested sub-directories,
you should uncheck the Recurse checkbox. (<strong>Note:</strong>
Don't forget to check your custom tag templates. They reside in
the CustomTags directory under the root of your ColdFusion
installation.)</p>

<p>The syntax checker will return a list of the ColdFusion pages that
failed the syntax checks and the error message that each failed
page generated.</p>

<p><b>Note:</b> running the syntax checker against large directories can take time and, in some cases, the browser may timeout. Currently, reducing the size of template directories is the only workaround, but an option to direct the checker output to a file is under development for a future release.</p>


<CFIF not isAllowedToExecute>
	<p>In order to prevent unauthorized execution of time-consuming 
	operations on your server, the CFML Syntax Checker may only be used from the local
	server IP <B>127.0.0.1</B>. If you are working on the server, use this IP reference
	to link to the CFML Syntax Checker."</p>
</CFIF>




<cfif isAllowedToExecute>

	<!--- Establish common parameter set --->
	<cfif isDefined("url.execute")>
		<cfparam name=directory default=#form.directory#>
		<cfparam name=filter default=#form.filter#>
		<cfif isDefined("form.recurse")>
			<cfset recurse = true>
		<cfelse>
			<cfset recurse = false>
		</cfif>	
	<cfelse>
		<cfparam name=directory default="">
		<cfparam name=filter default="*.cfm">
		<cfparam name=recurse default=true>
	</cfif>
	
	<!--- Display processing form --->
<br><br>
	
	<form action="cfmlsyntaxcheck.cfm?execute=yes&requestTimeout=1800" method="POST">
	
	<table>
		
	<cfoutput>
	
	<tr>
		<td>Directory:</td>
		<td><input type="Text" name="Directory" value="#directory#" size="60"></td>
	</tr>
	
	<tr>
		<td>Filter:</td>
		<td><input type="Text" name="Filter" value="#filter#" size="12"></td>
	</tr>
	
	<tr>
		<td>Recurse:</td>
		<td><input type="Checkbox" name="Recurse" <cfif recurse>checked</cfif>></td>
	</tr>
	
	</cfoutput>
	</table>
	<br><br>
	
	<input type="reset">
	<input type="Submit" name="Submit" value="Check Syntax">
	</form>
	
	
	
	<!--- Display result set --->
	<cfif isDefined("url.execute")>
	
		<hr><p><h2>Syntax Check Results</h2><p>
		
		<cf_syntaxcheck
			directory=#directory#
			filter=#filter#
			recurse=#recurse#
			resultquery=resultQuery>
	
		<cfif resultQuery.recordCount gt 0>
		
			<CFOUTPUT QUERY=resultQuery>
				<hr color="ff0000">
				Template Path: <b>#TemplatePath#</b><p>
				#ErrorMessage#
			</CFOUTPUT>
		
		<cfelse>
		
			<b>All templates comply with CFML 4.5 syntax rules!</b>	
		
		</cfif>
	</cfif>
	</font>

</cfif>




</BODY>
</HTML>
