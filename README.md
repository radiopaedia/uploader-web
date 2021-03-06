<h1>Radiopaedia Uploader: </h1><h2>WEB UI</h2>


<p>
Created by <a href="mailto:andyle2k@gmail.com">Andy Le</a> for <a href="https://www.radiopaedia.org">Radiopaedia.org</a>
</p>
Web Plugins:
<ul>        
    <li><a href="http://jquery.org">JQuery v1.11.3</a> (included)</li>
    <li><a href="http://getbootstrap.com">Bootstrap v3.3.6</a> (included)</li>
    <li><a href="https://github.com/allmarkedup/jQuery-URL-Parser">PURL v2.3.1</a> (included)</li>
    <li><a href="http://www.coolite.com/">JS Date 1.0 Alpha-1 </a> (included)</li>
    <li><a href="https://github.com/CodeSeven/toastr">Toastr</a> (included)</li>
    <li><a href="http://fancyapps.com/fancybox/">fancyBox 2.1.5</a> (included)</li>
    <li><a href="https://github.com/fengyuanchen/cropper">Cropper v2.3.2</a> (included)</li>
    <li><a href="http://datatables.net/">DataTables 1.10.11</a> (included)</li>
</ul>
<br />
ASP.NET (C#) Plugins/Libraries:
<ul>
    <li><a href="https://github.com/ClearCanvas">ClearCanvas DICOM libraries</a> <b>*(compile your own)</b></li>
    <li><a href="http://www.toptensoftware.com/petapoco/">PetaPoco v5.1.153</a> (included as NuGet package)</li>
    <li><a href="http://www.newtonsoft.com/json">Newtonsoft.Json v8.0.3</a> (included as NuGet package)</li>
</ul>
<b>*IMPORTANT* ClearCanvas</b>
<br />
You must download the ClearCanvas source files and compile your own DICOM libraries in order for the app to interact with PACS and read DICOM files.
<br />    
Notes:    
<ul>
    <li>
        Was written completely in <a href="https://www.visualstudio.com/vs/community/">Visual Studio 2015 Community Edition</a> (free)
    </li>
    <li>
        All JavaScript coding is written directly on the HTML files and are not in their own source files, this should make it easier to test changes.
    </li>
    <li>
        This app requires the database and the Agent to be running first before this web app can be deployed
    </li>
</ul>   

<br>
<p>
Further detailed instructions can be found on the manual here:<br>
<a href="https://github.com/radiopaedia/uploader-agent/blob/master/Radiopaedia%20Uploader%20Manual%2020161129_1609.pdf">
https://github.com/radiopaedia/uploader-agent/blob/master/Radiopaedia%20Uploader%20Manual%2020161129_1609.pdf
</a>
</p>
