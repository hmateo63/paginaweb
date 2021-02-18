<%@ Page Title="" Language="VB" MasterPageFile="~/privado/MasterPage.master" AutoEventWireup="false"
    CodeFile="DashBoard.aspx.vb" Inherits="privado_DashBoard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <style>
        .carousel-inner > .item > img, .carousel-inner > .item > a > img
        {
            width: 50%;
            margin: auto;
        }
    </style>



    <%--    <style type="text/css">
        body, html
        {
            height: 100%;
            background-repeat: no-repeat;
            background: url(../img/login-bg.jpg) no-repeat center center fixed;
        }
    </style>--%>


    <div id="myCarousel" class="carousel slide" data-ride="carousel">
        <!-- Indicators -->
        <ol class="carousel-indicators">
            <li data-target="#myCarousel" data-slide-to="0" class="active"></li>

<%--            <li data-target="#myCarousel" data-slide-to="1"></li>--%>

            <%--      <li data-target="#myCarousel" data-slide-to="2"></li>
      <li data-target="#myCarousel" data-slide-to="3"></li>
            --%>
        </ol>
        <!-- Wrapper for slides -->
        <div class="carousel-inner" role="listbox">
            <div class="item active">





                <img src="<%   =Session("carouselImagen") %>" alt="<%=Session("carouselEmpresa")%>" width="460" height="345">
                <div class="carousel-caption">
                    <h3><%=Session("carouselEmpresa")%></h3>
                    <p><%=Session("carouselMensaje")%></p>
                </div>
            </div>

<%--            <div class="item">
                <img src="img_vilma.jpg" alt="T Vilma" width="460" height="345">
                <div class="carousel-caption">
                    <h3>
                        Transportes Vilma</h3>
                    <p>
                        Chiquimula, Jocotan, El Florido</p>
                </div>
            </div>
--%>


<%--            <div class="item">
                <img src="img_flower.jpg" alt="Flower" width="460" height="345">
                <div class="carousel-caption">
                    <h3>
                        Flowers</h3>
                    <p>
                        Beautiful flowers in Kolymbari, Crete.</p>
                </div>
            </div>
            <div class="item">
                <img src="img_flower2.jpg" alt="Flower" width="460" height="345">
                <div class="carousel-caption">
                    <h3>
                        Flowers</h3>
                    <p>
                        Beautiful flowers in Kolymbari, Crete.</p>
                </div>
            </div>--%>

        </div>
        <!-- Left and right controls -->
        <a class="left carousel-control" href="#myCarousel" role="button" data-slide="prev">
            <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span><span class="sr-only">
                Previous</span> </a><a class="right carousel-control" href="#myCarousel" role="button"
                    data-slide="next"><span class="glyphicon glyphicon-chevron-right" aria-hidden="true">
                    </span><span class="sr-only">Next</span> </a>
    </div>
</asp:Content>
