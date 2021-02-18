<%@ Page Language="VB" AutoEventWireup="false" CodeFile="cuadre_caja_impresion.aspx.vb" Inherits="cierredecaja" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
          
<title>MiBoleto.com</title>
<meta charset="utf-8">

<link rel="stylesheet" href="css/style.css" type="text/css" media="all">
<%--<script type="text/javascript" src="js/jquery-1.4.2.js" ></script>
<script type="text/javascript" src="js/cufon-yui.js"></script>
<script type="text/javascript" src="js/cufon-replace.js"></script>  
<script type="text/javascript" src="js/Myriad_Pro_600.font.js"></script>--%>
<style type="text/css">

    
    table {
    *border-collapse: collapse; /* IE7 and lower */
    border-spacing: 0;
                width:85%;
            min-width:85%;
            margin-left:7%;
            position:absolute;    
}

.bordered {
    border: solid #ccc 1px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    border-radius: 6px;
    -webkit-box-shadow: 0 1px 1px #ccc; 
    -moz-box-shadow: 0 1px 1px #ccc; 
    box-shadow: 0 1px 1px #ccc;     
}
      
.bordered td, .bordered th {
    border-left: 1px solid #ccc;
    border-top: 1px solid #ccc;
    padding: 2px;
    
}

.bordered th {
    background-color: #dce9f9;
    background-image: -webkit-gradient(linear, left top, left bottom, from(#ebf3fc), to(#dce9f9));
    background-image: -webkit-linear-gradient(top, #ebf3fc, #dce9f9);
    background-image:    -moz-linear-gradient(top, #ebf3fc, #dce9f9);
    background-image:     -ms-linear-gradient(top, #ebf3fc, #dce9f9);
    background-image:      -o-linear-gradient(top, #ebf3fc, #dce9f9);
    background-image:         linear-gradient(top, #ebf3fc, #dce9f9);
    -webkit-box-shadow: 0 1px 0 rgba(255,255,255,.8) inset; 
    -moz-box-shadow:0 1px 0 rgba(255,255,255,.8) inset;  
    box-shadow: 0 1px 0 rgba(255,255,255,.8) inset;        
    border-top: none;
    text-shadow: 0 1px 0 rgba(255,255,255,.5); 
}

.bordered td:first-child, .bordered th:first-child {
    border-left: none;
}

.bordered th:first-child {
    -moz-border-radius: 6px 0 0 0;
    -webkit-border-radius: 6px 0 0 0;
    border-radius: 6px 0 0 0;
}

.bordered th:last-child {
    -moz-border-radius: 0 6px 0 0;
    -webkit-border-radius: 0 6px 0 0;
    border-radius: 0 6px 0 0;
}

.bordered th:only-child{
    -moz-border-radius: 6px 6px 0 0;
    -webkit-border-radius: 6px 6px 0 0;
    border-radius: 6px 6px 0 0;
}

.bordered tr:last-child td:first-child {
    -moz-border-radius: 0 0 0 6px;
    -webkit-border-radius: 0 0 0 6px;
    border-radius: 0 0 0 6px;
}

.bordered tr:last-child td:last-child {
    -moz-border-radius: 0 0 6px 0;
    -webkit-border-radius: 0 0 6px 0;
    border-radius: 0 0 6px 0;
}

.td{text-align:left;}

        .tdnumero
        {

            background-attachment: fixed;
            background-color:White;
            padding: 1px 8px 0px 3px;
            text-align:right;
            color:Black;
            text-align:right;
            font-size:13px;
            font-family:Palatino Linotype
        }
        .tdalign
        {
            border-width: 1px 1px 1px 1px;
            background-attachment: fixed;
            border-width: 1px;
            padding: 1px 8px 4px 3px;
            border-style: none;
            width: 26.3%;
        }
        .tdpanel
        {
            border-width: 1px 1px 1px 1px;
            background-attachment: fixed;
            border-width: 1px;
            padding: 1px 8px 5px 3px;
            border-style: none;
        }
        .total
        {
            border-style:solid;
            padding-top:10px;
            font-size:medium;
            margin-left:120px;
            color:black
                      
        }
        .Encabezado
        {
            background-color:#BDBDBD;
            font-size:12px;
            font-family:Palatino Linotype;
            color:black;
            border-style:solid;
            border-width:1.5px;
            font-size:100%;
            font-style:bold;          
        }
         .h1
        {
            color: black;
            margin-left:9%;
            position:absolute;
            top:-10px;
            z-index:3;
            left:75%;
            border-style:solid;
            height:100px;
            border-width:4px;
            width:30px;
            font-size:80%;
            
            
        }
          .h2 
        {
         font-size:40px;
         text-transform:uppercase;
         font-weight:600;
         color:#0B6121;
         padding:2px 0 10px 0;
         text-align:center;
         font-size:14px;       
         font-family:Palatino Linotype
                 
        }
         .print
        {         
            border-width: 1px 1px 1px 1px;
            margin: 2px 2px 2px 2px;
            border-style: none;
            color: black;
            width:80%;
            min-width:300px;
            margin-left:6%;
            position:absolute;
            top:90%;
            z-index:3;          
        }
         
        </style>
<!--[if lt IE 9]>
	<script type="text/javascript" src="http://info.template-help.com/files/ie6_warning/ie6_script_other.js"></script>
	<script type="text/javascript" src="js/html5.js"></script>
<![endif]-->
</head>
<body id="page2">


<form id="form1" runat="server">



<!-- header -->
		
				
				
		

 <div style="height:300px; width:100%">
 
 
     </div>

</form>

</body>
</html>
