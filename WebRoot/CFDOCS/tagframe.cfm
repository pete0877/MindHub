<HTML>
<HEAD>

<CFParam Name = "CGI.Host" default="">
<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>
	<CFINCLUDE template="/cfdocs/sorry.htm">
	<CFABORT>

</CFIF>

<CFIF Left(tagname, 2) EQ "cf">
  <CFOUTPUT><TITLE>CFML Tag Example: #UCase(tagname)#</TITLE></CFOUTPUT>
<CFELSE>
	<CFOUTPUT><TITLE>CFML Function Example: #tagname#</TITLE></CFOUTPUT>
</CFIF>

</head>


<CFSET filename = "#tagname#">

<CFSET trimmedTagName = lcase(Trim(#tagname#))> 

<CFSET filename = ExpandPath("snippets/#trimmedTagName#.cfm")>


<CFIF FileExists(filename)>


		<CFOUTPUT>
			<frameset rows="14%, *" onLoad=window.resizeTo(820,800)>
				<frame frameborder="yes" 
					src="snippets/exampleheader.cfm?tagname=#tagname#" 
					NAME="ExampleHeader">
				</frame>
			
				<frameset cols="50%,50%">
					<frame frameborder="yes" 
						src="snippets/viewexample.cfm?tagname=#tagname#" 
						NAME="ViewExample">
					</frame>
			
					<frame frameborder="yes" 
						src="snippets/runexample.cfm?tagname=#tagname#" 
						NAME="RunExample">
					</frame>

				</frameset>

			</frameset>

		</CFOUTPUT>

<CFELSE>



	<frameset rows="100%, *" onLoad=window.resizeTo(550,600)>
		<frame frameborder="yes" 
			src="sorry.htm" 
			name="sorry">
		</frame>
	</frameset>	 


</CFIF>
</HTML>
