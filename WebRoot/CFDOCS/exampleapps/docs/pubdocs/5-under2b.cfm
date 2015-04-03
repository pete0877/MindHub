<CFSET Title = "Under the Hood, Pt. 2 Continued">
<CFINCLUDE TEMPLATE="_header.cfm">

<h3>Under the Hood, Pt. II Continued</h3>

<P>Now that you've seen how <CODE>&lt;CF_ShowObject&gt;</CODE> handles <I>explicit</I> calls,
let's see how <CODE>&lt;CF_ShowContent&gt;</CODE> handles <I>implicit</I> calls.</P>

<H4>&lt;CF_ShowContent&gt;</H4>

<P>The first thing <CODE>&lt;CF_ShowContent&gt;</CODE> has to do is figure out what
page it's on, what location it's dealing with, and what instances should be shown
for that page, location, and time.</P>

<P>First, the function <CODE>GetTemplatePath()</CODE> is used to determine what 
page we're dealing with. If the page is correctly registered in the database,
the query GetPageID will have one (and only one) record.</P>

<BLOCKQUOTE><PRE>&lt;CFQUERY DATASOURCE=&quot;CFexamples&quot; NAME=&quot;GetPageID&quot;&gt;
SELECT * FROM PubPages
WHERE TemplatePath = '#GetFileFromPath(GetTemplatePath())#'
&lt;/CFQUERY&gt;

&lt;CFIF GetPageID.RecordCount IS 0&gt;
    &lt;CFSETTING ENABLECFOUTPUTONLY=&quot;NO&quot;&gt;
    &lt;CFOUTPUT&gt;&lt;!-- (CF_ShowContent) ERROR: PageID not found!! --&gt;&lt;/CFOUTPUT&gt;
    &lt;CFEXIT&gt;
&lt;/CFIF&gt;</PRE></BLOCKQUOTE>

<P>Then we use CFPARAM to default the Location number to 1 (if none was specified).</P>

<BLOCKQUOTE><PRE>&lt;CFPARAM NAME="Attributes.Location" DEFAULT="1"&gt;</PRE></BLOCKQUOTE>

<P>Now we're ready to query the database and see exactly which instances should
appear (and in what order). Notice that it's possible to use the URL variable 
CurrentTime to see how the site would look at a different time.</P>

<BLOCKQUOTE><PRE>&lt;CFPARAM NAME=&quot;URL.CurrentTime&quot; DEFAULT=&quot;#Now()#&quot;&gt;

&lt;CFQUERY DATASOURCE=&quot;CFexamples&quot; NAME=&quot;GetInstances&quot;&gt;
SELECT PubObjects.*, PubInstances.*, PubDataClasses.*
FROM PubObjects, PubInstances, PubDataClasses
WHERE PubObjects.ObjectID = PubInstances.ObjectID
    AND PubObjects.ClassID = PubDataClasses.ClassID
    AND PageID = #GetPageID.PageID#
    AND (StartTime &lt; #CreateODBCDateTime(URL.CurrentTime)# OR StartTime = Null)
    AND (EndTime &gt; #CreateODBCDateTime(URL.CurrentTime)# OR EndTime = Null)
    AND Location = #Attributes.Location#
ORDER BY Priority DESC
&lt;/CFQUERY&gt;</PRE></BLOCKQUOTE>

<P>At this point, the query GetInstances contains all of the instances that 
should appear. Now we loop over the query and do the same thing we did for
<CODE>&lt;CF_ShowObject&gt;</CODE>: query for the object's content, pour it
into an associative array, then render the HTML.</P>

<BLOCKQUOTE><PRE>&lt;CFLOOP QUERY=&quot;GetInstances&quot;&gt;

    &lt;CFQUERY DATASOURCE=&quot;CFexamples&quot; NAME=&quot;GetContent&quot;&gt;
    SELECT PubContent.*, PubContentTypes.*
    FROM PubContent, PubContentTypes
    WHERE PubContent.TypeID = PubContentTypes.TypeID
        AND ObjectID = #GetInstances.ObjectID#
    &lt;/CFQUERY&gt;

    &lt;CFSET CurrObject = StructNew()&gt;
    &lt;CFLOOP QUERY=&quot;GetContent&quot;&gt;
        &lt;CFSET Temp = StructInsert(CurrObject, TypeName, Data)&gt;
    &lt;/CFLOOP&gt;
    &lt;CFSET CurrObject.ClassName = ClassName&gt;
    &lt;CFSET CurrObject.ObjectID = ObjectID&gt;

    &lt;CFPARAM NAME=&quot;Attributes.ViewMode&quot; DEFAULT=&quot;default&quot;&gt;
    &lt;CFINCLUDE TEMPLATE=&quot;viewmode/#Attributes.ViewMode#.cfm&quot;&gt;
        
&lt;/CFLOOP&gt;</PRE></BLOCKQUOTE>

<P>Note that this time, the code to render the HTML isn't stored within
the custom tag itself, but instead in templates in the ViewMode subdirectory.
Using the ViewMode custom tag attribute, you can choose different templates.
So if you had a ViewMode/TopStory.cfm template that showed the news item in a larger
font, you could use the following code:</P>

<BLOCKQUOTE><PRE>&lt;CF_ShowContent Location=&quot;1&quot; ViewMode=&quot;TopStory&quot;&gt;
&lt;CF_ShowContent Location=&quot;2&quot;&gt;</PRE></BLOCKQUOTE>

<P>For the purposes of this example app, we've only included the default ViewMode;
the code for it appears below, and should be pretty self-explanatory.</P>

<BLOCKQUOTE><PRE>&lt;CFOUTPUT&gt;
&lt;P&gt;&lt;DIV CLASS=&quot;HeadlineTeaser&quot;&gt;#CurrObject.Headline#&lt;BR&gt;&lt;/DIV&gt;
&lt;DIV CLASS=&quot;TeaserTeaser&quot;&gt;#CurrObject.Teaser#&lt;BR&gt;&lt;/DIV&gt;

&lt;CFIF CurrObject.ClassName IS 'News Item'&gt;
    &lt;DIV CLASS=&quot;LinkTeaser&quot;&gt;&lt;A HREF=&quot;viewfull.cfm?ObjectID=#CurrObject.ObjectID#&quot;&gt;More Info&lt;/A&gt;&lt;/DIV&gt;
&lt;CFELSEIF CurrObject.ClassName IS 'File with Description'&gt;
    &lt;DIV CLASS=&quot;LinkTeaser&quot;&gt;&lt;A HREF=&quot;binarydata/#Replace(Replace(URLEncodedFormat(CurrObject.File),&quot;%2E&quot;,&quot;.&quot;,&quot;ALL&quot;),&quot;+&quot;,&quot;%20&quot;,&quot;ALL&quot;)#&quot;&gt;Download File&lt;/A&gt;&lt;/DIV&gt;
&lt;CFELSEIF CurrObject.ClassName IS 'Hyperlink'&gt;
    &lt;DIV CLASS=&quot;LinkTeaser&quot;&gt;&lt;A HREF=&quot;#CurrObject.HREF#&quot;&gt;Go to page&lt;/A&gt;&lt;/DIV&gt;
&lt;/CFIF&gt;

&lt;!--- If browser is in Admin mode, display editing icons ---&gt;
&lt;CFIF IsDefined(&quot;Cookie.PubAdminMode&quot;)&gt;
    &lt;A HREF=&quot;admin/properties.cfm?ObjectID=#ObjectID#&quot;&gt;&lt;IMG SRC=&quot;open.gif&quot; WIDTH=16 HEIGHT=14 BORDER=0 ALT=&quot;Open&quot; ALIGN=&quot;TOP&quot;&gt;&lt;/A&gt;
    &lt;A HREF=&quot;admin/deleteinstance.cfm?InstanceID=#InstanceID#&quot;&gt;&lt;IMG SRC=&quot;delete.gif&quot; WIDTH=15 HEIGHT=16 BORDER=0 ALT=&quot;Delete&quot; ALIGN=&quot;TOP&quot;&gt;&lt;/A&gt;
&lt;/CFIF&gt;

&lt;/CFOUTPUT&gt;</PRE></BLOCKQUOTE>

<CFSET HREF = "6-learnmore.cfm">
<CFSET Link = "Learning More">
<CFINCLUDE TEMPLATE="_footer.cfm">