<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %>

<!DOCTYPE html>
<html>
<head>
    <title>Sign-Up/Login Form</title>
    <link href='https://fonts.googleapis.com/css?family=Titillium+Web:400,300,600' rel='stylesheet'
        type='text/css'>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/normalize/5.0.0/normalize.min.css">
    <link rel="stylesheet" href="css/loginstyle.css">
</head>
<body>
    <div class="form">
        <div class="tab-content">
            <div id="signup">
                <h1>
                </h1>
                <form runat="server" id="form">
                <div class="field-wrap">

<%--                    <label>                        Usuario<span class="req">*</span>                    </label>
--%>
                    <input type="text" required autocomplete="off" id="TxtUser" name="TxtUser" runat="server" />
                </div>
                <div class="field-wrap">

<%--                    <label>
                        Password<span class="req">*</span>
                    </label>
--%>
                    <input type="password" required autocomplete="off" id="TxtPass" name="TxtPass" runat="server" />
                </div>
                <div>
                    <asp:Button ID="BtnLogin" runat="server" Text="Conectar" class="button button-block" />
                    <asp:Label ID="LblEstado" runat="server" Text=""></asp:Label>
                </div>
                <div id="divcliente" runat="server" visible="false">
                    <asp:TextBox ID="TxtNavegador" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TxtSo" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TxtVersion" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TxtUserAgent" runat="server" Style="height: 22px"></asp:TextBox>

                </div>
                </form>
            </div>
            <div id="login">
                <h1>
                    Welcome Back!</h1>
                <form action="/" method="post">
                <div class="field-wrap">

<%--                    <label>
                        Email Address<span class="req">*</span>
                    </label>
--%>
                    <input type="email" required autocomplete="off" />
                </div>
                <div class="field-wrap">

<%--                    <label>
                        Password<span class="req">*</span>
                    </label>
--%>
                    <input type="password" required autocomplete="off" />
                </div>
                <p class="forgot">
                    <a href="#">Forgot Password?</a></p>
                <button class="button button-block" />
                Log In</button>
                </form>
            </div>
        </div>
        <!-- tab-content -->
    </div>
    <!-- /form -->
    <script src='http://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js'></script>
    <script src="js/index.js"></script>
</body>
</html>
