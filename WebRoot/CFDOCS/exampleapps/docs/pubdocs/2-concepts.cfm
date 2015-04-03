<CFSET Title = "DCP Concepts">
<CFINCLUDE TEMPLATE="_header.cfm">

<h3>DCP Concepts</h3>
<h4>Objects, Classes, and Content</h4>
<P>The content publishing model in this example application is based on the idea 
  of <I>objects</I>. An object is a piece of information or a group of related 
  information. For example, the title of a press release, an executive summary 
  of the press release, the body of the press release, and a Word document of 
  the press release would all be pieces of content contained in the same Press 
  Release object.</P>
<P>Depending on the way the object is called, the content can be displayed in 
  different ways. A News Summary page might only display titles of news objects, 
  but clicking on one of those titles could call a page which shows all of the 
  content of the object.</P>
<P>Each object must be of a certain <I>class</I>, such as News Items and Press 
  Releases. The class of an object allows pages to determine how objects should 
  be formatted when they are displayed.</P>
<P>So far, each <I>object</I> has pieces of <I>content</I>, and every <I>object</I> 
  belongs in a certain <I>class</I>. Each piece of content also has a <I>content 
  type</I>, such as Headline, Teaser (executive summary), Body, File, Image, etc. 
  As with object classes, content types allow pages to determine how content should 
  be formatted when they are displayed.</P>
<P>Not all content types are allowed for all object classes; for example, all 
  News Items are required to contain a Headline, Teaser, and Body; they may optionally 
  contain a File, Image, or Hyperlink to be displayed along with the image; and 
  they may not contain an Address or Phone Number. These relationships are defined 
  in the database table PubClassDefinitions.</P>
<h4>Implicit/Explicit Calls, Instances, and Pages</h4>
<P>There are two ways in which objects can be displayed. The first way is through 
  an <i>explicit call</i>; i.e., a page contains code that says "Display the object 
  with ObjectID 23 here." The second way is through an <i>implicit call</i>; the 
  page says "See if any objects are supposed to be displayed on this page, in 
  this spot, at this time; if so, display them in the proper order."</P>
<P>The way objects respond to implicit calls is through <I>instances</I>. If the 
  user wants object 23 to appear on three different pages in the web site, three 
  separate instances must be created.</P>
<P>In order for instances to be scheduled for a page, that page must be registered 
  with the system, and given its own PageID. Then, when the page is called, it 
  finds the instances that match its PageID (and other criteria) and displays 
  the objects associated with each instance.</P>

<CFSET HREF="3-under1.cfm">
<CFSET Link="Under the Hood, Pt. 1">
<CFINCLUDE TEMPLATE="_footer.cfm">