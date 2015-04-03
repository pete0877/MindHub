<CFSET Title = "Under the Hood, Pt. 2">
<CFINCLUDE TEMPLATE="_header.cfm">

<h3>Under the Hood, Pt. II</h3>
<P>So far, we've described explicit and implicit object calls in very abstract 
  terms; now you'll see how it is actually implemented.</P>
<P>Each type of object call is wrapped in a custom tag. Explicit calls are executed 
  via <CODE>&lt;CF_ShowObject&gt;</CODE> and implicit calls are executed via 
  <CODE>&lt;CF_ShowContent&gt;</CODE>.</P>
<h4>&lt;CF_ShowObject&gt;</H4>
<P><CODE>&lt;CF_ShowObject&gt;</CODE> is the simpler of the two tags because it
  only needs to render one object, and it already knows which object it wants. We
  start with a <CODE>&lt;CFQUERY&gt;</CODE> to fetch all of the object's content
  from the database.</P>
  
<BLOCKQUOTE><PRE>&lt;CFQUERY DATASOURCE=&quot;CFexamples&quot; NAME=&quot;GetContent&quot;&gt;
SELECT PubContent.*, PubContentTypes.*
FROM PubContent, PubContentTypes
WHERE PubContent.TypeID = PubContentTypes.TypeID
    AND ObjectID = #Attributes.ObjectID#
&lt;/CFQUERY&gt;</PRE></BLOCKQUOTE>

<P>This query gives us a recordset that looks something like this:</P>

<TABLE BORDER="1" CELLPADDING="4" CELLSPACING="0">
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>ObjectID</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Type</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Data</B></FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">132</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Title</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"This Story's Title"</FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">132</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Teaser</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"Read this interesting article"</FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">132</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Body</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"The main story would go here."</FONT></TD>
</TR>
</TABLE>


<P>Then, we create an associative array (or "struct") and put the content into it.
As you will see, this makes the data far easier to work with later on.</P>

<BLOCKQUOTE><PRE>&lt;CFSET Object = StructNew()&gt;
&lt;CFLOOP QUERY=&quot;GetContent&quot;&gt;
    &lt;CFSET Temp = StructInsert(Object, TypeName, Data)&gt;
&lt;/CFLOOP&gt;
&lt;CFSET Object.ObjectID = ObjectID&gt;</PRE></BLOCKQUOTE>

<P>Now we have an associative array that looks like this:</P>

<TABLE BORDER="1" CELLPADDING="4" CELLSPACING="0">
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Key</B></FONT></TD>
    <TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Value</B></FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">ObjectID</FONT></TD>
    <TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">132</FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Title</FONT></TD>
    <TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"This Story's Title"</FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Teaser</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"Read this interesting article"</FONT></TD>
</TR>
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Body</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">"The main story would go here."</FONT></TD>
</TR>
</TABLE>

<P>With Object array in hand, we are ready to render the object to HTML. For the purposes
of this example application, we have put the CFML that renders the object directly into
the <CODE>&lt;CF_ShowObject&gt;</CODE> tag, so you'd have to edit the tag itself if you
wanted to create multiple ways of viewing objects. (<CODE>&lt;CF_ShowContent&gt;</CODE> does
a little of this; see that section to learn more.)</P>

<BLOCKQUOTE><PRE>&lt;CFOUTPUT&gt;

    &lt;DIV CLASS=&quot;HeadlineFull&quot;&gt;#Object.Headline#&lt;/DIV&gt;

    &amp;nbsp;&lt;BR&gt;

    &lt;CFIF IsDefined(&quot;Object.Image&quot;)&gt;
        &lt;P&gt;&lt;IMG SRC=&quot;binarydata/#Object.Image#&quot;&gt;
    &lt;/CFIF&gt;

    &lt;CFIF IsDefined(&quot;Object.Body&quot;)&gt;&lt;DIV CLASS=&quot;BodyFull&quot;&gt;
        &lt;CFIF IsDefined(&quot;Object.InlineImage&quot;)&gt;
            &lt;IMG SRC=&quot;binarydata/#Object.InlineImage#&quot; ALIGN=&quot;LEFT&quot; HSPACE=&quot;10&quot;&gt;
        &lt;/CFIF&gt;
        #Object.Body#&lt;/DIV&gt;
    &lt;/CFIF&gt;

    &amp;nbsp;&lt;BR&gt;

    &lt;CFIF IsDefined(&quot;Object.HREF&quot;)&gt;
        &lt;DIV CLASS=&quot;LinkFull&quot;&gt;&lt;B&gt;Go to:&lt;/B&gt; &lt;A HREF=&quot;#Object.HREF#&quot;&gt;#Object.HREF#&lt;/A&gt;&lt;BR&gt;&lt;/DIV&gt;
    &lt;/CFIF&gt;
    &lt;CFIF IsDefined(&quot;Object.File&quot;)&gt;
        &lt;DIV CLASS=&quot;LinkFull&quot;&gt;&lt;B&gt;Download:&lt;/B&gt; &lt;A HREF=&quot;binarydata/#Replace(Replace(URLEncodedFormat(Object.File),&quot;%2E&quot;,&quot;.&quot;,&quot;ALL&quot;),&quot;+&quot;,&quot;%20&quot;,&quot;ALL&quot;)#&quot;&gt;#Object.File#&lt;/A&gt;&lt;BR&gt;&lt;/DIV&gt;
    &lt;/CFIF&gt;

    &lt;!--- If browser is in Admin mode, display editing icons ---&gt;
    &lt;CFIF IsDefined(&quot;Cookie.PubAdminMode&quot;)&gt;
        &lt;BR&gt;&lt;A HREF=&quot;admin/properties.cfm?ObjectID=#ObjectID#&quot;&gt;&lt;IMG SRC=&quot;open.gif&quot; WIDTH=16 HEIGHT=14 BORDER=0 ALT=&quot;Open&quot; ALIGN=&quot;TOP&quot;&gt;&lt;/A&gt;
        &lt;A HREF=&quot;admin/deleteobject.cfm?ObjectID=#ObjectID#&quot;&gt;&lt;IMG SRC=&quot;delete.gif&quot; WIDTH=15 HEIGHT=16 BORDER=0 ALT=&quot;Delete&quot; ALIGN=&quot;TOP&quot;&gt;&lt;/A&gt;
    &lt;/CFIF&gt;

&lt;/CFOUTPUT&gt;</PRE></BLOCKQUOTE>

<CFSET HREF = "5-under2b.cfm">
<CFSET Link = "Under the Hood, Pt. 2 Continued">
<CFINCLUDE TEMPLATE="_footer.cfm">