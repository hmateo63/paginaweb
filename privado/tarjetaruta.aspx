<%@ Page Language="VB" AutoEventWireup="false" CodeFile="tarjetaruta.aspx.vb" Inherits="privado_tarjetaruta" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title>LITEGUA</title>
    <link href="../css/styles.css" rel="stylesheet" type="text/css" />
    <link href="source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="css/notifications.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery1.8.3.js" type="text/javascript"></script>
    <script src="js/jquery1.92.2.js" type="text/javascript"></script>
    <script src="source/jquery.fancybox.pack.js" type="text/javascript"></script>
    <script src="js/jquery.notifications.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/plugins/charts/excanvas.min.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.orderBars.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.pie.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.resize.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.sparkline.min.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.sortable.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.resizable.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.autosize.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.uniform.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.inputlimiter.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.tagsinput.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.maskedinput.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.autotab.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.select2.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.dualListBox.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.cleditor.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.ibutton.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.validationEngine-en.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.validationEngine.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.html4.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.html5.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/jquery.plupload.queue.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.form.wizard.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.validate.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.form.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.collapsible.min.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.breadcrumbs.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.tipsy.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.progress.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.timeentry.min.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.colorpicker.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.jgrowl.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.fileTree.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.sourcerer.js"></script>
    <script type="text/javascript" src="../js/plugins/others/jquery.fullcalendar.js"></script>
    <script type="text/javascript" src="../js/plugins/others/jquery.elfinder.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.mousewheel.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.easytabs.min.js"></script>
    <script type="text/javascript" src="../js/files/bootstrap.js"></script>
    <script type="text/javascript" src="../js/files/functions.js"></script>
    <style type="text/css">
        .mostrar
        {
            position: relative;
            float: left;
            width: 350px;
        }
        .ocultar
        {
            position: relative;
            float: left;
            width: 350px;
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        var usuario = ""
        var numerodetalles = 0;
        var origenes = "";
        var origenestipocobroruta = "";
        var contaclick = 0;
        function mensajecorrecto(mensaje) {
            $.jGrowl(mensaje, { theme: 'success', position: 'bottom-right' });
        }

        function mensajeadvertencia(mensaje) {
            $.jGrowl(mensaje, { theme: 'warning', position: 'bottom-right' });
        }
        function mensajeerror(mensaje) {
            $.jGrowl(mensaje, { theme: 'error', position: 'bottom-right' });
        }

        function mensajeerrorpermanente(mensaje) {
            $.jGrowl(mensaje, { theme: 'error', position: 'bottom-right', sticky: true });
        }

        $(function () {
            $("#txtfechaviaje").datepicker({
                numberOfMonths: 1,
                dateFormat: 'dd/mm/yy',
                firstDay: 0
            });
        });


        //INICIO DOCUMENT READY
        $(document).ready(function () {

            if (document.getElementById('husuario').value == "") {
                mensajeerrorpermanente("No se ha obtenido el dato del Usuario");
                return false;
            } else {
                usuario = document.getElementById('husuario').value;
                document.getElementById('usuario').innerHTML = usuario;
            }

            cargarOrigen('droporigen');
            cargarSerie();

            cargarTipoCobroRuta('droptipocobroruta');



            // ultimo correlativo de esta serie
            $("#dropserie").blur(function () {
                $(this).css("background-color", "#FFFFCC");
            });

            $("#btnguardarboletosagencia").click(function () {

                var correlativo = document.getElementById('hcorrelativo').value;
                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);
                var idboleto = document.getElementById('txtboletosagencia').value;

                if (usuario == "") {

                    parent.location.href = '../login.aspx';
                }
                else {

                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/registraBoletosAgencia",
                        data: '{numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + ',idboleto: "' + idboleto + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function () {
                            $("#cargar1").removeClass("ocultar");
                            $("#cargar1").addClass("mostrar");
                        },
                        complete: function () {
                            $("#cargar1").removeClass("mostrar");
                            $("#cargar1").addClass("ocultar");
                        },
                        success: mostrarboletosagencia,
                        error: mostrarboletosagenciano
                    });
                }
            });

            function mostrarboletosagencia(msg) {

                contaclick = 0;
                var respuesta = msg.d;

                if (respuesta.substring(0, 5) == "ERROR") {
                    mensajeerror(respuesta);
                }
                else {
                    var correlativo = document.getElementById('hcorrelativo').value;
                    muestraBoletosvendidos(correlativo);
                    mensajecorrecto(respuesta);
                    document.getElementById('txtboletosagencia').value = "";
                }


            }

            function mostrarboletosagenciano(msg) { contaclick = 0; mensajeerrorpermanente(msg.responseText); }



            $("#btnliberarhorario").click(function () {

                var correlativo = document.getElementById('hcorrelativo').value;
                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);
                var idboleto = document.getElementById('txtboletosagencia').value;

                if (usuario == "") {

                    parent.location.href = '../login.aspx';
                }
                else {

                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/LiberarHorarioBoletosAgencia",
                        data: '{numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + ',usuario: "' + usuario + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function () {
                            $("#cargar1").removeClass("ocultar");
                            $("#cargar1").addClass("mostrar");
                        },
                        complete: function () {
                            $("#cargar1").removeClass("mostrar");
                            $("#cargar1").addClass("ocultar");
                        },
                        success: mostrarliberarhorario,
                        error: mostrarliberarhorariono
                    });
                }
            });

            function mostrarliberarhorario(msg) {

                var respuesta = msg.d;

                if (respuesta.substring(0, 5) == "ERROR") {
                    mensajeerror(respuesta);
                }
                else {
                    var correlativo = document.getElementById('hcorrelativo').value;
                    muestraBoletosvendidos(correlativo);
                    mensajecorrecto(respuesta);
                    document.getElementById('txtboletosagencia').value = "";
                    $("#filaencabezado").html('');
                    $("#detalle").html('');
                    $("#drophorarioguardado").html('');
                    $("#filaingresodetalle").html('');
                    $("#btnbuscar").enable(true);
                    $("#txtnumerotarjeta").val('');
                    $("#txtnumerotarjeta").enable(true);
                    $("#pie").html('');

                    setTimeout(function () { window.location.reload() }, 3000);


                }


            }

            function mostrarliberarhorariono(msg) { mensajeerrorpermanente(msg.responseText); }


            $("#btnguardarboletos").click(function () {

                document.getElementById('btnguardarboletos').style.display = 'none';


                var contador = 0;
                //FACTURA
                var correlativo = 0;
                var Factura = new Array();
                var FacturaDetalle = new Array();
                var Boleto = new Array();
                var dropserieventa = document.getElementById('dropserieventa');

                var seriegeneral = dropserieventa.options[dropserieventa.selectedIndex].text;
                arrayseriefactura = seriegeneral.split("|");
                var seriefactura = arrayseriefactura[0];
                var error = 0;
                //CORRELATIVO DE LOS DATOS SELECCIONADOS ====DETALLE DE BOLETOS EN RUTA====//
                var hcorrelativo = document.getElementById('hcorrelativo').value;

                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + hcorrelativo);




                $("#detalleboletos .formRow").each(function (index) {
                    valor = document.getElementById('txtvalor' + contador);

                    if (!valor.value || isNaN(valor.value)) { mensajeadvertencia("Dato inválido"); valor.focus(); error = 1; return false; }

                    var numerofactura = document.getElementById('txtboleto' + contador);


                    // var fechafactura = document.getElementById('txtfechaviaje');
                    var fechafactura = "";
                    var horafactura = "";
                    var valorfactura = valor.value;
                    var nitcliente = ""; var nombrecliente = ""; var idagencia = 0; var idusuario = ""; var idcaja = 0; var cae = ""; var face = ""; var recargo = 0;
                    var efectivo = parseFloat(valor.value);
                    var fechacreacion = "";
                    var tarjetacredito = 0; var cheque = 0; var referencia = ""; var numeroimpresion = 0; var estadocobro = ""; var idempresa = 0; var acreditado = 0;


                    // enviar para 
                    var descripcionorigen = document.getElementById('droporigen' + contador);
                    var myIdorigen = descripcionorigen.Id;
                    var myValueorigen = descripcionorigen.value;
                    var myTextorigen = descripcionorigen.options[descripcionorigen.selectedIndex].text;

                    var descripciondestino = document.getElementById('dropdestino' + contador);
                    var myIddestino = descripciondestino.Id;
                    var myValuedestino = descripciondestino.value;
                    var myTextdestino = descripciondestino.options[descripciondestino.selectedIndex].text;

                    var descripciontipocobroruta = document.getElementById('droptipocobroruta' + contador);
                    var myIdtipocobroruta = descripciontipocobroruta.Id;
                    var myValuetipocobroruta = descripciontipocobroruta.value;
                    var myTexttipocobroruta = descripciontipocobroruta.options[descripciontipocobroruta.selectedIndex].text;

                    var descripcionenviar = "ORIGEN: " + myTextorigen + " / DESTINO: " + myTextdestino;

                    if (myValuetipocobroruta > 0) {
                        var descripcionenviar = myTexttipocobroruta;
                    }



                    //ARREGLO FACTURA
                    Factura.push({ correlativo: correlativo, seriefactura: seriefactura, numerofactura: numerofactura.value,
                        fechafactura: fechafactura,
                        horafactura: horafactura,
                        valorfactura: valorfactura,
                        nitcliente: nitcliente,
                        nombrecliente: nombrecliente,
                        idagencia: idagencia,
                        idusuario: idusuario,
                        idcaja: idcaja,
                        cae: cae,
                        face: face,
                        recargo: recargo,
                        efectivo: efectivo,
                        tarjetacredito: tarjetacredito,
                        cheque: cheque,
                        referencia: referencia,
                        numeroimpresion: numeroimpresion,
                        estadocobro: estadocobro,
                        idempresa: idempresa,
                        acreditado: acreditado
                    });
                    var item = 3;
                    var boleto = 0;

                    FacturaDetalle.push({ correlativo: correlativo,
                        numerofactura: numerofactura.value,
                        seriefactura: seriefactura,
                        item: item,
                        boleto: boleto,
                        valor: valor.value,
                        descripcion: descripcionenviar
                    });

                    //var arreglohorario = drophorario.value.split("|");

                    var fechaviaje = document.getElementById('txtfechahorario' + hcorrelativo);



                    var droporigen = document.getElementById('droporigen' + contador);
                    var dropdestino = document.getElementById('dropdestino' + contador);
                    var seguro = 0;
                    var tipocobro = 2;
                    var numerobutaca = 0;
                    var estadobutaca = "";
                    var estadofacturacion = "";
                    var estadoboleto = "";
                    var saldo = 0;
                    var Descuento = 0;
                    var AutorizaDescuento = "";
                    var recargo = 0;
                    var credito = "";
                    var boletocredito = "";
                    var idhora = document.getElementById('hidhora' + hcorrelativo);
                    var idruta = document.getElementById('hidruta' + hcorrelativo);
                    var secuenciaorigen = 0;
                    var secuenciadestino = 0;
                    var idservicio = document.getElementById('hidservicio' + hcorrelativo); ;
                    var IdCliente = 0;
                    var idtipoboleto = 3;
                    var idestadoboleto = 1;
                    var tipo_cobro = 2;

                    Boleto.push({ idboleto: boleto, fechacreacion: fechacreacion,
                        fechaviaje: fechaviaje.value,
                        origen: droporigen.value,
                        destino: dropdestino.value,
                        nitcliente: nitcliente,
                        seguro: seguro,
                        tipo_cobro: tipo_cobro,
                        numerobutaca: numerobutaca,
                        estadobutaca: estadobutaca,
                        idempresa: idempresa,
                        idagencia: idagencia,
                        idusuario: idusuario,
                        estadofacturacion: estadofacturacion,
                        estadoboleto: estadoboleto,
                        saldo: saldo,
                        total: valor.value,
                        Descuento: Descuento,
                        AutorizaDescuento: AutorizaDescuento,
                        recargo: recargo,
                        credito: credito,
                        boletocredito: boletocredito,
                        idhora: idhora.value,
                        idruta: idruta.value,
                        secuenciaorigen: secuenciaorigen,
                        secuenciadestino: secuenciadestino,
                        idservicio: idservicio.value,
                        IdCliente: IdCliente,
                        idtipoboleto: idtipoboleto,
                        idestadoboleto: idestadoboleto
                    });
                    contador++;

                });

                /*console.log(Factura);
                console.log(FacturaDetalle);
                console.log(Boleto);*/

                if (error > 0) {
                    mensajeadvertencia("Ha sucedido un error al momento de enviar los datos"); return;
                }
                else if (contador <= 0) {
                    mensajeadvertencia("No se recuperaron los datos ingresados"); return;
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/GuardaBoleto",
                        data: "{'Factura':" + JSON.stringify(Factura) + ",'Facturadetalle':" + JSON.stringify(FacturaDetalle) + ",'Boleto':" + JSON.stringify(Boleto) + ",'idagenciacaja':" + dropserieventa.value + ",'numerotarjeta':" + numerotarjeta.value + ",'idtarjetadetalle':" + idtarjetadetalle.value + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function () {
                            $("#cargarboletos").removeClass("ocultar");
                            $("#cargarboletos").addClass("mostrar");
                        },
                        complete: function () {
                            $("#cargarboletos").removeClass("mostrar");
                            $("#cargarboletos").addClass("ocultar");
                        },
                        success: guardaboletos,
                        error: guardaboletosno
                    });
                }
            });

            function guardaboletos(msg) {
                var resultado = msg.d;
                //CORRELATIVO DE LOS DATOS SELECCIONADOS ====DETALLE DE BOLETOS EN RUTA====//

                var correlativo = document.getElementById('hcorrelativo').value;
                if (resultado.substring(0, 5) == "ERROR") { mensajeadvertencia(resultado); }
                else {
                    mensajecorrecto(resultado);
                    $("#detalleboletos").html("");

                    muestraBoletosvendidos(correlativo);

                    //$.fancybox.update();
                    //muestraUltimoBoleto(hcorrelativo);
                }
            }

            function guardaboletosno(msg) { mensajeerrorpermanente(msg.responseText); }



            $("#btnguardarcarga").click(function () {
                var contador = 0;
                //FACTURA
                var correlativo = 0;
                var Factura = new Array();
                var FacturaDetalle = new Array();
                var Boleto = new Array();
                var dropserieventa = document.getElementById('dropseriecarga');

                var seriegeneral = dropserieventa.options[dropserieventa.selectedIndex].text;
                arrayseriefactura = seriegeneral.split("|");
                var seriefactura = arrayseriefactura[0];
                var error = 0;
                //CORRELATIVO DE LOS DATOS SELECCIONADOS ====DETALLE DE BOLETOS EN RUTA====//
                var hcorrelativo = document.getElementById('hcorrelativo').value;

                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + hcorrelativo);


                $("#detallecarga .formRow").each(function (index) {
                    valor = document.getElementById('txtvalor' + contador);
                    if (!valor.value || isNaN(valor.value)) { mensajeadvertencia("Dato inválido"); valor.focus(); error = 1; return false; }
                    var numerofactura = document.getElementById('txtfacturacarga' + contador);
                    var fechafactura = ""; var horafactura = "";
                    var valorfactura = valor.value;
                    var nitcliente = ""; var nombrecliente = ""; var idagencia = 0; var idusuario = ""; var idcaja = 0; var cae = ""; var face = ""; var recargo = 0;
                    var efectivo = parseFloat(valor.value);
                    var fechacreacion = "";
                    var tarjetacredito = 0; var cheque = 0; var referencia = ""; var numeroimpresion = 0; var estadocobro = ""; var idempresa = 0; var acreditado = 0;
                    //ARREGLO FACTURA
                    Factura.push({ correlativo: correlativo, seriefactura: seriefactura, numerofactura: numerofactura.value,
                        fechafactura: fechafactura,
                        horafactura: horafactura,
                        valorfactura: valorfactura,
                        nitcliente: nitcliente,
                        nombrecliente: nombrecliente,
                        idagencia: idagencia,
                        idusuario: idusuario,
                        idcaja: idcaja,
                        cae: cae,
                        face: face,
                        recargo: recargo,
                        efectivo: efectivo,
                        tarjetacredito: tarjetacredito,
                        cheque: cheque,
                        referencia: referencia,
                        numeroimpresion: numeroimpresion,
                        estadocobro: estadocobro,
                        idempresa: idempresa,
                        acreditado: acreditado
                    });
                    var item = 3;
                    var boleto = 0;

                    FacturaDetalle.push({ correlativo: correlativo, numerofactura: numerofactura.value, seriefactura: seriefactura,
                        item: item,
                        boleto: boleto,
                        valor: valor.value
                    });
                    //var arreglohorario = drophorario.value.split("|");

                    var fechaviaje = document.getElementById('txtfechahorario' + hcorrelativo);
                    var droporigen = 0
                    var dropdestino = 0
                    var seguro = 0;
                    var tipocobro = 2;
                    var numerobutaca = 0;
                    var estadobutaca = "";
                    var estadofacturacion = "";
                    var estadoboleto = "";
                    var saldo = 0;
                    var Descuento = 0;
                    var AutorizaDescuento = "";
                    var recargo = 0;
                    var credito = "";
                    var boletocredito = "";
                    var idhora = 0
                    var idruta = 0
                    var secuenciaorigen = 0;
                    var secuenciadestino = 0;
                    var idservicio = 0
                    var IdCliente = 0;
                    var idtipoboleto = 3;
                    var idestadoboleto = 1;
                    var tipo_cobro = 2;

                    Boleto.push({ idboleto: boleto, fechacreacion: fechacreacion,
                        fechaviaje: fechaviaje.value,
                        origen: droporigen.value,
                        destino: dropdestino.value,
                        nitcliente: nitcliente,
                        seguro: seguro,
                        tipo_cobro: tipo_cobro,
                        numerobutaca: numerobutaca,
                        estadobutaca: estadobutaca,
                        idempresa: idempresa,
                        idagencia: idagencia,
                        idusuario: idusuario,
                        estadofacturacion: estadofacturacion,
                        estadoboleto: estadoboleto,
                        saldo: saldo,
                        total: valor.value,
                        Descuento: Descuento,
                        AutorizaDescuento: AutorizaDescuento,
                        recargo: recargo,
                        credito: credito,
                        boletocredito: boletocredito,
                        idhora: idhora.value,
                        idruta: idruta.value,
                        secuenciaorigen: secuenciaorigen,
                        secuenciadestino: secuenciadestino,
                        idservicio: idservicio.value,
                        IdCliente: IdCliente,
                        idtipoboleto: idtipoboleto,
                        idestadoboleto: idestadoboleto
                    });
                    contador++;

                });

                /*console.log(Factura);
                console.log(FacturaDetalle);
                console.log(Boleto);*/

                if (error > 0) {
                    mensajeadvertencia("Ha sucedido un error al momento de enviar los datos"); return;
                }
                else if (contador <= 0) {
                    mensajeadvertencia("No se recuperaron los datos ingresados"); return;
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/GuardaCarga",
                        data: "{'Factura':" + JSON.stringify(Factura) + ",'Facturadetalle':" + JSON.stringify(FacturaDetalle) + ",'Carga':" + JSON.stringify(Boleto) + ",'idagenciacaja':" + dropseriecarga.value + ",'numerotarjeta':" + numerotarjeta.value + ",'idtarjetadetalle':" + idtarjetadetalle.value + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        beforeSend: function () {
                            $("#cargarboletos").removeClass("ocultar");
                            $("#cargarboletos").addClass("mostrar");
                        },
                        complete: function () {
                            $("#cargarboletos").removeClass("mostrar");
                            $("#cargarboletos").addClass("ocultar");
                        },
                        success: guardacarga,
                        error: guardacargano
                    });
                }
            });

            function guardacarga(msg) {
                var resultado = msg.d;
                //CORRELATIVO DE LOS DATOS SELECCIONADOS ====DETALLE DE BOLETOS EN RUTA====//

                var correlativo = document.getElementById('hcorrelativo').value;
                if (resultado.substring(0, 5) == "ERROR") { mensajeadvertencia(resultado); }
                else {
                    mensajecorrecto(resultado);
                    $("#detallecarga").html("");

                    muestraCargaVendida(correlativo);

                    //$.fancybox.update();
                    //muestraUltimoBoleto(hcorrelativo);
                }
            }

            function guardacargano(msg) { mensajeerrorpermanente(msg.responseText); }




            $("#btncancelar").click(function () {
                $("#filaencabezado").fadeOut('slow');
                $("#filaingresodetalle").fadeOut('slow');
                $('#detalle').fadeOut('slow');
                setTimeout("$('#detalle').html('');", 500);
                setTimeout("$('#detalle').fadeIn('slow');", 1000);

                document.getElementById('hcorrelativo').value = 0;

                $('#pie').fadeOut('slow');
                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var botonbuscar = document.getElementById('btnbuscar');
                numerotarjeta.value = "";
                numerotarjeta.disabled = false;
                botonbuscar.disabled = false;
                botonbuscar.value = "Buscar"
                numerotarjeta.focus();
            });

            $("#btngenerarseries").click(function () {
                var serie = document.getElementById('dropserieventa');
                var ultimocorrelativo = document.getElementById('txtultimocorrelativo');

                if (!serie.value) { mensajeadvertencia("Seleccione un numero de serie"); serie.focus(); return; }
                else if (!ultimocorrelativo.value) { mensajeadvertencia("Ingrese el ultimo numero correlativo vendido"); ultimocorrelativo.focus(); return; }
                else if (isNaN(ultimocorrelativo.value) == true) { mensajeadvertencia("Ingrese un número"); ultimocorrelativo.focus(); return; }
                else if (ultimocorrelativo.value < 1) { mensajeadvertencia("Ingrese un número mayor a 0"); ultimocorrelativo.focus(); return; }
                else {
                    busquedaserie(serie, ultimocorrelativo);
                }
            });


            $("#btngenerarseriescarga").click(function () {
                var serie = document.getElementById('dropseriecarga');
                var ultimocorrelativo = document.getElementById('txtultimocorrelativocarga');

                if (!serie.value) { mensajeadvertencia("Seleccione un numero de serie"); serie.focus(); return; }
                else if (!ultimocorrelativo.value) { mensajeadvertencia("Ingrese el ultimo numero correlativo vendido"); ultimocorrelativo.focus(); return; }
                else if (isNaN(ultimocorrelativo.value) == true) { mensajeadvertencia("Ingrese un número"); ultimocorrelativo.focus(); return; }
                else if (ultimocorrelativo.value < 1) { mensajeadvertencia("Ingrese un número mayor a 0"); ultimocorrelativo.focus(); return; }
                else {
                    busquedaseriecarga(serie, ultimocorrelativo);
                }
            });






            $("#btncerrartarjeta").click(function () {
                var numerotarjeta = document.getElementById('txtnumerotarjeta');

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/cerrarTarjeta",
                    data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idtarjetaestado: 2}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: cerrartarjeta,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: cerrartarjetano
                });

            });

            function cerrartarjeta(msg) {
                $.each(msg.d, function () {
                    if (this.correcto == false) {
                        mensajeerrorpermanente(this.mensaje);
                    } else {
                        mensajecorrecto(this.mensaje);
                        mostrarbusqueda();
                    }
                });
            }

            function cerrartarjetano(msg) { mensajeerrorpermanente(msg.responseText); }

            $("#btnbuscar").click(function () {
                var accion = $(this).val();
                if (accion == "Crear") {
                    crearTarjeta();
                    $("#drophorarioguardado").html('');

                }
                else if (accion == "Buscar") {
                    var numerotarjeta = document.getElementById('txtnumerotarjeta');
                    if (!numerotarjeta.value) { mensajeadvertencia("Ingrese numero de tarjeta"); numerotarjeta.focus(); return; }
                    buscarTarjeta(numerotarjeta.value);
                }
            });

            $("#btnagregartalonario").click(function () {

                //limpiar

                //$("#dropserie").html("");


                document.getElementById('txtpilotofirma').value = document.getElementById('txtpiloto1').value;

                muestraSeries()

                var servicio = document.getElementById('txtservicio1').value;

                //alert(servicio);

                abrimodal('talonario');

            });


            $("#btnentregartalonario").click(function () {
                document.getElementById('txtpilotofirma2').value = document.getElementById('txtpiloto1').value;
                muestraSeries2()
                abrimodal('talonario2');
            });



            // btnvaleelectronico INICIA
            $("#btnvaleelectronico").click(function () {
                abrimodal('valeselectronico');



                var numerotarjetavale = document.getElementById('txtnumerotarjeta').value;

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/TotalValeElectronico",
                    data: '{NumeroTarjeta:' + numerotarjetavale + ',Usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: totalvaleelectronico,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: totalvaleelectronicono
                });

                function totalvaleelectronico(msg) {
                    var codigohtml = "";

                    codigohtml = "<div class='formRow'><div class='gridvale' style='border-color: transparent;'> <h6></h6></div><div class='gridvale' style=' text-align: right; font-size: 1.6em; width: 95.5%;'> <h6><input type='button' id='btnguardarvaleelectronico' onclick='guardarvaleelectronico();' class='buttonS bLightBlue' value='Generar Vale De Ruta'> </h6></div></div>";

                    $("#divvaleelectronico").fadeOut("slow");
                    $("#divvaleelectronico").html('');
                    $("#divvaleelectronico").append(msg.d);
                    $("#divvaleelectronico").append(codigohtml);
                    $("#divvaleelectronico").fadeIn("slow");


                    $("#btnguardarvaleelectronico").click(function () {

                        document.getElementById("btnguardarvaleelectronico").disabled = "disabled";

                        $.ajax({
                            type: "POST",
                            url: "WStarjetaruta.asmx/GuardarValeElectronico",
                            data: '{NumeroTarjeta:' + numerotarjetavale + ',Usuario: "' + usuario + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: guardavaleelectronico,
                            beforeSend: function () {
                                $("#cargar").removeClass("ocultar");
                                $("#cargar").addClass("mostrar");
                            },
                            complete: function () {
                                $("#cargar").removeClass("mostrar");
                                $("#cargar").addClass("ocultar");
                            },
                            error: guardavaleelectronicoerror
                        });

                    });

                    function guardavaleelectronico(msg) {
                        var numerotarjetavale = document.getElementById('txtnumerotarjeta').value;
                        cerrarmodal('valeselectronico');

                        var respuesta = msg.d;

                        if (respuesta.substring(0, 5) == "ERROR") {
                            mensajeerror(respuesta);
                        }
                        else {
                            abremodal(respuesta);
                        }



                        //                    valepdf(numerotarjetavale);
                    }


                    function guardavaleelectronicoerror(msg) { mensajeerrorpermanente(msg.responseText); }



                }

                function totalvaleelectronicono(msg) { mensajeerrorpermanente(msg.responseText); }



            });

            // btnvaleelectronico FIANALIZA




            // BTNVALE INICIA
            $("#btnvale").click(function () {
                abrimodal('vales');



                var numerotarjetavale = document.getElementById('txtnumerotarjeta').value;

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/TotalVale",
                    data: '{NumeroTarjeta:' + numerotarjetavale + ',Usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: totalvale,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: totalvaleno
                });

                function totalvale(msg) {
                    var codigohtml = "";

                    codigohtml = "<div class='formRow'><div class='gridvale' style='border-color: transparent;'> <h6></h6></div><div class='gridvale' style=' text-align: right; font-size: 1.6em; width: 95.5%;'> <h6><input type='button' id='btnguardarvale' onclick='guardarvale();' class='buttonS bLightBlue' value='Generar Vale De Ruta'> </h6></div></div>";

                    $("#divvale").fadeOut("slow");
                    $("#divvale").html('');
                    $("#divvale").append(msg.d);
                    $("#divvale").append(codigohtml);
                    $("#divvale").fadeIn("slow");


                    $("#btnguardarvale").click(function () {

                        document.getElementById("btnguardarvale").disabled = "disabled";

                        $.ajax({
                            type: "POST",
                            url: "WStarjetaruta.asmx/GuardarVale",
                            data: '{NumeroTarjeta:' + numerotarjetavale + ',Usuario: "' + usuario + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: guardavale,
                            beforeSend: function () {
                                $("#cargar").removeClass("ocultar");
                                $("#cargar").addClass("mostrar");
                            },
                            complete: function () {
                                $("#cargar").removeClass("mostrar");
                                $("#cargar").addClass("ocultar");
                            },
                            error: guardavaleerror
                        });

                    });

                    function guardavale(msg) {
                        var numerotarjetavale = document.getElementById('txtnumerotarjeta').value;
                        cerrarmodal('vales');


                        var respuesta = msg.d;

                        if (respuesta.substring(0, 5) == "ERROR") {
                            mensajeerror(respuesta);
                        }
                        else {
                            abremodal(respuesta);
                        }



                        //                    valepdf(numerotarjetavale);
                    }


                    function guardavaleerror(msg) { mensajeerrorpermanente(msg.responseText); }



                }

                function totalvaleno(msg) { mensajeerrorpermanente(msg.responseText); }



            });

            // BTNVALE FIANALIZA



            $("#btnasignarhorario").click(function () {

                cargarTurno();

                var vehiculo = document.getElementById('DropVehiculo').value;
                var fecha = document.getElementById('txtfechaviaje');
                var droporigen = document.getElementById('droporigen');
                var drophorario = document.getElementById('drophorario');
                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var fechaviaje = document.getElementById('txtfechaviaje').value;



                if ($('#drophorario option').length <= 0) {

                    mensajeadvertencia("No hay horarios asociados a esta fecha")

                }

                $("#drophorario option").each(function () {

                    var arrayhorario = $(this).attr('value').split("|");



                    if (!fecha.value) {
                        mensajeadvertencia('No ha ingresado una fecha');
                        fecha.focus();
                        return;
                    }
                    else if (droporigen.selectedIndex < 0) {

                        mensajeadvertencia('No ha seleccionado un origen');
                        droporigen.focus();
                        return;
                    }
                    else if (drophorario.selectedIndex < 0) {
                        mensajeadvertencia('No ha seleccionado un horario valido');
                        drophorario.focus();
                        return;
                    }
                    else {


                        $.ajax({
                            type: "POST",
                            url: "WStarjetaruta.asmx/asignarHorario",
                            data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idhorario: ' + arrayhorario[0] + ',vehiculo: ' + vehiculo + ',fechaviaje:"' + fechaviaje + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: asignarhorario,
                            beforeSend: function () {
                                $("#cargaragregarhorarios").removeClass("ocultar");
                                $("#cargaragregarhorarios").addClass("mostrar");
                            },
                            complete: function () {
                                $("#cargaragregarhorarios").removeClass("mostrar");
                                $("#cargaragregarhorarios").addClass("ocultar");
                            },
                            error: asignarhorariono
                        });
                    }

                });
            });

            function asignarhorario(msg) {

                var contador = 0;
                var codigohtml = "";
                var codigohtmltabla = "";
                var mensaje = 0;
                $("#drophorario option").each(function () {

                    var value = $(this).attr('value').split("|")
                    codigohtml = codigohtml + '<option value="' + value[0] + '">' + $(this).text() + '</option>';
                    codigohtmltabla = codigohtmltabla + '<tr><td>' + $(this).text().split("|") + '</td></tr>'
                    contador++;
                });

                if (contador > 0) {



                    $("#drophorarioguardado").html("");
                    $("#drophorarioguardado").html(codigohtml);
                    $("#dhorariosasignados").html("");
                    $("#dhorariosasignados").append(codigohtmltabla);



                    $("#titulohorarios").html('');
                    $("#titulohorarios").append("Agregar horarios de vehiculo a Tarjeta de ruta" + " <p style='color:green'>  Horarios Guardados Correctamente</p>");
                    $("#btnasignarhorario").css('display', 'inline');
                    document.getElementById("DropVehiculo").disabled = "disabled";
                    document.getElementById("txtfechaviaje").disabled = "disabled";
                    setTimeout("$.fancybox.update();", 500);

                    var vehiculo = document.getElementById('DropVehiculo').value;
                    $("#titulo").html("");
                    $("#titulo").append("TARJETA DE RUTA DEL VEHICULO : " + vehiculo);
                    $("#titulo").css('color', '#52777b');

                    document.getElementById('btnasignarhorario').value = "Actualizar Horarios"
                }
                else {
                    $("#titulohorarios").html('');
                    $("#titulohorarios").append("Agregar horarios de vehiculo a Tarjeta de ruta" + " <p style='color:red'> Error: Los Horarios Para Esta Fecha Ya Estan Registrados</p>");
                }
            }

            function asignarhorariono(msg) { mensajeerrorpermanente(msg.responseText); }

            $("#btnagregarhorario").click(function () {

                abrimodal('agregarhorarios');
                setTimeout("document.getElementById('txtfechaviaje').focus();", 1000);
                cargarvehiculos();

                if (document.getElementById('txtfechaviaje').value == "") {
                    document.getElementById('txtfechaviaje').value = document.getElementById('txtfechacreacione').value;
                }

            });





            function cargarvehiculos() {


                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/CargarVehiculo",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) {
                        Vehiculos = msg.d;
                        $.each(msg.d, function () {
                            $('#DropVehiculo').append('<option value="' + this.idvehiculo + '">' + this.nombrevehiculo + '</option>');
                        });
                    },
                    error: function (msg) { alert("Error " + msg.responseText); }
                });





            }

            $("#btnregresar").click(function () {
                $("#tresumenhorariose").fadeIn("slow");
                $("#tmontos").fadeOut("slow");
                document.getElementById('btnregresar').style.display = 'none';
                setTimeout("$.fancybox.update();", 1000);

            });

            $("#btnhorarios").click(function () {
                muestraHorarios()
                $("#tmontos").fadeOut("slow");
                $("#tresumenhorariose").fadeIn("slow");
                document.getElementById('btnregresar').style.display = 'none';
                abrimodal('horarios');
            });

            $("#btnagregar").click(function () {
                var drophorario = document.getElementById('drophorarioguardado');
                var carga = document.getElementById('txtcarga');
                var excursion = document.getElementById('txtexcursion');
                var viaticospiloto = document.getElementById('txtviaticospiloto');
                var viaticosasistente = document.getElementById('txtviaticosasistente');
                var numerovale = document.getElementById('txtidvale');
                var numerotarjetap = document.getElementById('txtnumerotarjeta');

                if (!carga.value) carga.value = 0;
                if (!excursion.value) excursion.value = 0;
                if (!viaticospiloto.value) viaticospiloto.value = 0;
                if (!viaticosasistente.value) viaticosasistente.value = 0;
                if (!numerovale.value) numerovale.value = 0;

                if (drophorario.selectedIndex < 0) {
                    mensajeadvertencia('No ha seleccionado un horario valido');
                    drophorario.focus();
                    return;
                }
                else {
                    //var arreglohorario = drophorario.value.split("|");
                    //var horario = arreglohorario[0];
                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/crearTarjetadetalle",
                        data: '{numerotarjetap: ' + numerotarjetap.value + ',usuario: "' + usuario + '",idhorariop: ' + drophorario.value + ',cargap: ' + carga.value + ',excursionp: ' + excursion.value + ',viaticospilotop: ' + viaticospiloto.value + ',viaticosasistentep: ' + viaticosasistente.value + ',numerovalep: ' + numerovale.value + '}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: agregardetalle,
                        beforeSend: function () {
                            $("#cargar").removeClass("ocultar");
                            $("#cargar").addClass("mostrar");
                        },
                        complete: function () {
                            $("#cargar").removeClass("mostrar");
                            $("#cargar").addClass("ocultar");
                        },
                        error: agregardetalleno
                    });
                }

            });

            $("#btnresumen").click(function () {
                var numerotarjetap = document.getElementById('txtnumerotarjeta');

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/ResumenTarjetaPdf",
                    data: '{numerotarjeta: ' + numerotarjetap.value + ',usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: resumenboletos,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: resumenboletosno
                });
            });

            function resumenboletos(msg) {
                var respuesta = msg.d;

                if (respuesta.substring(0, 5) == "ERROR") {
                    mensajeerror(respuesta);
                }
                else {
                    abremodal(respuesta);
                }
            }

            function resumenboletosno(msg) { mensajeerrorpermanente(msg.responseText); }

            $('#txtfechaviaje').bind("blur change focus", function () {

                cargarTurno();

            });



            //  el resumen de boletos

            $("#btntarjetatalonario_liquidarfacturaspdf").click(function () {
                var numerotarjetap = document.getElementById('txtnumerotarjeta');

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/TarjetaTalonario_LiquidarFacturasPdf",
                    data: '{numerotarjeta: ' + numerotarjetap.value + ',usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: resumenboletosderutaboletos,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: resumenboletosderutaboletosno
                });
            });

            function resumenboletosderutaboletos(msg) {
                var respuesta = msg.d;

                if (respuesta.substring(0, 5) == "ERROR") {
                    mensajeerror(respuesta);
                }
                else {
                    abremodal(respuesta);
                }
            }

            function resumenboletosderutaboletosno(msg) { mensajeerrorpermanente(msg.responseText); }

            $('#txtfechaviaje').bind("blur change focus", function () {

                cargarTurno();

            });



            $("#btntarjetatalonario_entregafacturaspdf").click(function () {
                var numerotarjetap = document.getElementById('txtnumerotarjeta');

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/TarjetaTalonario_EntregaFacturasPdf",
                    data: '{numerotarjeta: ' + numerotarjetap.value + ',usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: resumenentregafacturas,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: resumenentregafacturasno
                });
            });

            function resumenentregafacturas(msg) {
                var respuesta = msg.d;

                if (respuesta.substring(0, 5) == "ERROR") {
                    mensajeerror(respuesta);
                }
                else {
                    abremodal(respuesta);
                }
            }

            function resumenentregafacturasno(msg) { mensajeerrorpermanente(msg.responseText); }

            $('#txtfechaviaje').bind("blur change focus", function () {

                cargarTurno();

            });




            //            $('#dropserie').bind("blur change focus", function () {

            //                alert("que onda");

            //            });








            // ------------------------ ASIGNAR SERIE

            ///   cambiare

            $('#btnasignarserie').click(function () {
                var dropserie = document.getElementById('dropserie');
                var numerotarjeta = document.getElementById('txtnumerotarjeta');
                var correlativoinicial = document.getElementById('txtcorrelativoinicial').value;
                var correlativofinal = document.getElementById('txtcorrelativofinal').value;
                var nombrepiloto = document.getElementById('txtpiloto1').value;

                if (dropserie.selectedIndex < 1) {
                    mensajeadvertencia("Seleccione una serie para asignar a la Tarjeta");
                    dropserie.focus();
                    return;
                }

                //  voy a tratar de meter 

                if (correlativoinicial < 1) {
                    mensajeadvertencia("ingrese Correlativo Inicial");
                    txtcorrelativoinicial.focus();
                    return;
                }


                if (correlativofinal < 1) {
                    mensajeadvertencia("ingrese Correlativo Final");
                    txtcorrelativofinal.focus();
                    return;
                }


                var dropserievalue = dropserie.options[dropserie.selectedIndex].text;
                var arreglo = dropserievalue.split("|");
                var separar = arreglo[1];

                // ME QUEDO CON LA SEGUNDA COLUMNA 
                var arreglo2 = separar.split("-");

                var x = parseInt(arreglo2[0]);
                var y = parseInt(arreglo2[1]);

                if (correlativoinicial >= x && correlativoinicial <= y) {

                    var todobien = 1;

                } else {
                    mensajeadvertencia("Correlativo Inicial incorrecto");
                    txtcorrelativoinicial.focus();
                    return;
                }

                if (correlativofinal >= x && correlativofinal <= y) {

                    var todobien = 2;

                } else {

                    mensajeadvertencia("Correlativo Final incorrecto");
                    txtcorrelativofinal.focus();
                    return;
                }

                if (correlativoinicial >= x && correlativoinicial <= y && correlativoinicial >= x && correlativoinicial <= y) {

                    var todobien = 3;

                } else {

                    mensajeadvertencia("Revise Correlativos ");
                    txtcorrelativofinal.focus();
                    return;
                }

                if (dropserie.selectedIndex < 1) {
                    mensajeadvertencia("Seleccione una serie para asignar a la Tarjeta");
                    dropserie.focus();
                    return;
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/asignarSerieTarjeta",
                        data: '{usuario: "' + usuario + '",numerotarjeta:' + numerotarjeta.value + ',idagenciacaja: ' + dropserie.value + ',correlativoinicial: ' + correlativoinicial + ',correlativofinal: ' + correlativofinal + ',nombrepiloto: "' + nombrepiloto + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: asignarserie,
                        error: asignarserieno
                    });
                }
            });


            //  boton de entrega
            // ------------------------ ASIGNAR SERIE

            $('#btnasignarserie2').click(function () {
                var dropserie = document.getElementById('dropserie2').value;
                // alert(dropserie);

                var numerotarjeta = document.getElementById('txtnumerotarjeta').value;
                //alert("tarjeta " + numerotarjeta);

                var correlativoinicial = document.getElementById('hidcorrelativoinicial2').value;
                //alert("correlativo inicial   " + correlativoinicial);

                var correlativofinal = document.getElementById('hidcorrelativofinal2').value;
                //alert("correlativo final   " + correlativofinal);


                var dropserievalue = dropserie2.options[dropserie2.selectedIndex].text;
                var arreglo = dropserievalue.split("|");
                var separar = arreglo[1];


                var utilizado = document.getElementById('txtutilizado').value;

                if (utilizado < 1) {
                    mensajeadvertencia("correlativo  utilizado 0");
                    txtutilizado.focus();
                    return;
                }

                // ME QUEDO CON LA SEGUNDA COLUMNA 
                var arreglo2 = separar.split("-");

                var x = parseInt(arreglo2[0]);
                var y = parseInt(arreglo2[1]);


                if (utilizado >= x && utilizado <= y) {

                    var todobien = 1;

                } else {
                    mensajeadvertencia("Correlativo utilizado incorrecto");
                    txtcorrelativoinicial2.focus();
                    return;
                }

                var nombrepiloto = document.getElementById('txtpilotofirma2').value;

                var valorfacturas = document.getElementById('txtvalorfacturas').value;

                if (valorfacturas < 1) {
                    mensajeadvertencia("Valor Facturas 0");
                    txtvalorfactura.focus();
                    return;
                }


                if (dropserie < 1) {
                    mensajeadvertencia("Seleccione una serie para asignar a la Tarjeta");
                    dropserie2.focus();
                    return;
                }
                else {

                    $.ajax({
                        type: "POST",
                        url: "WStarjetaruta.asmx/talonarioUtilizadosEntregar",
                        data: '{usuario: "' + usuario + '",numerotarjeta:' + numerotarjeta + ',idagenciacaja: ' + dropserie2.value + ',correlativoinicial: ' + correlativoinicial + ',correlativofinal: ' + correlativofinal + ',correlativoutilizado: ' + utilizado + ',valor: ' + valorfacturas + ',nombrepiloto: "' + nombrepiloto + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: asignarserie,
                        error: asignarserieno
                    });
                }
            });
        });
        //FIN DOCUMENT READY

        function abremodal(ruta) {
            $.fancybox.open({
                href: ruta,
                type: 'iframe',
                width: 1200,
                padding: 5
            });
        }

        function cargarHorarios() {

            var numerotarjeta = document.getElementById('txtnumerotarjeta');

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/mostrarHorarios",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: muestrahorarioscorrecto,
                beforeSend: function () {
                    $("#cargaragregarhorarios").removeClass("ocultar");
                    $("#cargaragregarhorarios").addClass("mostrar");
                },
                complete: function () {
                    $("#cargaragregarhorarios").removeClass("mostrar");
                    $("#cargaragregarhorarios").addClass("ocultar");
                },
                error: muestrahorariosno
            });
        }

        function muestrahorarioscorrecto(msg) {
            $("#drophorarioguardado").html("");
            $("#dhorariosasignados").html("");

            $.each(msg.d, function () {
                $('#drophorarioguardado').append('<option value="' + this.datos + '">' + this.hora + '</option>');
                $("#dhorariosasignados").append('<tr><td>' + this.hora + '</td></tr>');
            });

            //$("#dhorariosasignados").html("");
            //$("#dhorariosasignados").append(codigohtmltabla);
        }

        function muestrahorariosno(msg) { mensajeerrorpermanente(msg.responseText); }

        function redondea(number) {
            var newnumber = Math.round(number * Math.pow(10, 2)) / Math.pow(10, 2);
            return newnumber.toFixed(2);
        }

        function asignarserie(msg) {
            var resultado = "";
            $.each(msg.d, function () {
                if (this.correcto == true) {
                    mensajecorrecto(this.mensaje);
                    muestraSeries();
                }
                else {
                    mensajeadvertencia(this.mensaje);
                }
            });
        }

        function asignarserieno(msg) { mensajeerrorpermanente(msg.responseText); }




        function muestraBoletosvendidos(correlativo) {

            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraBoletosvendidos",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                success: mostrarboletos,
                error: mostrarboletosno
            });
        }


        function muestraCargaVendida(correlativo) {

            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraCargavendida",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                success: mostrarboletos,
                error: mostrarboletosno
            });
        }


        function muestraCargaAgenciaVendida(correlativo) {

            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);


            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraCargaAgenciaVendida",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                success: mostrarcargaagencia,
                error: mostrarcargaagencia
            });
        }
        function mostrarcargaagencia(msg) {

            var totalcarga = "";
            totalcarga = parseFloat(msg.d).toFixed(2)
            $("#tdcargaagencia").html(totalcarga);


            if (totalcarga > 0) {

                $("#BtnGuardarCargaAgencia").css('display', 'none');
                $("#TituloTotalCargaAgencia").css('display', 'none');
                $("#TxtTotalCargaAgencia").css('display', 'none');


            }

            else {

                $("#BtnGuardarCargaAgencia").css('display', 'block');
                $("#TituloTotalCargaAgencia").css('display', 'block');
                $("#TxtTotalCargaAgencia").css('display', 'block');

            }


        }



        function GuardarCargaAgencia(correlativo) {


            control = $("#numerocarga").html();
            var valor = document.getElementById(control).id;
            var totalcargaagencia = document.getElementById("TxtTotalCargaAgencia").value;
            correlativo = valor.substring(15, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;


            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            var idtarjetadetalle = document.getElementById('hidtarjetadetalle' + correlativo);


            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/GuardaCargaAgencia",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + ',idtarjetadetalle: ' + idtarjetadetalle.value + ',totalcarga: ' + totalcargaagencia + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                success: guardarcargaagencia,
                error: guardarcargaagencia
            });
        }
        function guardarcargaagencia(msg) {

            var respuesta = msg.d;

            if (respuesta.substring(0, 5) == "ERROR") {
                mensajeerror(respuesta);
                document.getElementById("TxtTotalCargaAgencia").value = "";
                control = $("#numerocarga").html();
                var valor = document.getElementById(control).id;
                var correlativo = valor.substring(15, valor.length);
                document.getElementById('hcorrelativo').value = correlativo;


                muestraCargaAgenciaVendida(correlativo);
            }
            else {
                mensajecorrecto(respuesta);
                document.getElementById("TxtTotalCargaAgencia").value = "";
                control = $("#numerocarga").html();
                var valor = document.getElementById(control).id;
                var correlativo = valor.substring(15, valor.length);
                document.getElementById('hcorrelativo').value = correlativo;


                muestraCargaAgenciaVendida(correlativo);
            }




        }






        function mostrarboletos(msg) {
            var codigohtmlruta = "";
            var codigohtmlagencia = "";

            $("#boletosguardados").html("");
            $("#boletosguardadosagencia").html("");
            $("#cargaguardada").html("");

            codigohtmlruta = '';
            codigohtmlagencia = '';
            codigohtmlcarga = '';

            $.each(msg.d, function () {



                if (this.item == 3) {
                    codigohtmlruta = codigohtmlruta + '<tr>' +
                        '<td>' + this.idboleto + '</td>' +
                        '<td>' + this.numerofactura + '</td>' +
                        '<td>' + this.serie + '</td>' +
                        '<td>' + this.estadoboleto + '</td>' +
                        '<td>' + this.idusuario + '</td>' +
                    //'<td>' + this.fechacreacion + '</td>' +
                        '<td>' + redondea(this.total) + '</td>' +
                    '</tr>';
                }

                else if (this.item == 2) {
                    codigohtmlagencia = codigohtmlagencia + '<tr>' +
                        '<td>' + this.idboleto + '</td>' +
                        '<td>' + this.numerofactura + '</td>' +
                        '<td>' + this.serie + '</td>' +
                        '<td>' + this.estadoboleto + '</td>' +
                        '<td>' + this.idusuario + '</td>' +
                    //'<td>' + this.fechacreacion + '</td>' +
                        '<td>' + redondea(this.total) + '</td>' +
                    '</tr>';
                }

                else if (this.item == 5) {
                    codigohtmlcarga = codigohtmlcarga + '<tr>' +
                        '<td>' + this.idcarga + '</td>' +
                        '<td>' + this.numerofactura + '</td>' +
                        '<td>' + this.serie + '</td>' +
                        '<td>' + this.estadocarga + '</td>' +
                        '<td>' + this.idusuario + '</td>' +
                    //'<td>' + this.fechacreacion + '</td>' +
                        '<td>' + redondea(this.total) + '</td>' +
                    '</tr>';
                }


            });

            $("#boletosguardados").append(codigohtmlruta);
            $("#boletosguardadosagencia").append(codigohtmlagencia);
            $("#cargaguardada").append(codigohtmlcarga);

            $.fancybox.update();
        }

        function mostrarboletosno(msg) { mensajeerrorpermanente(msg.responseText); }

        function muestraSeriesDrop() {
            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraSeries",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                success: mostrarseriesdrop,
                error: mostrarseriesdropno
            });
        }

        function mostrarseriesdrop(msg) {

            $.each(msg.d, function () {
                $('#dropserieventa').append('<option value="' + this.correlativo + '">' + this.serie + '</option>');
                $('#dropseriecarga').append('<option value="' + this.correlativo + '">' + this.serie + '</option>');
            });
        }

        function mostrarseriesdropno(msg) { mensajeerrorpermanente(msg.responseText); }

        function muestraSeries() {
            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraSeries",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar2").removeClass("ocultar");
                    $("#cargar2").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar2").removeClass("mostrar");
                    $("#cargar2").addClass("ocultar");
                },
                success: mostrarseries,
                error: mostrarseriesno
            });
        }

        function mostrarseries(msg) {
            var codigohtml = "";
            $("#detalletalonariosd").html("");
            codigohtml = '';

            $.each(msg.d, function () {
                codigohtml = codigohtml + '<tr>' +
                        '<td>' + this.serie + '</td>' +
                        '<td>' + this.usuario + '</td>' +
                        '<td>' + this.fechacreacion + '</td>' +
                        '<td>' + this.correlativoinicial + '</td>' +
                        '<td>' + this.correlativofinal + '</td>' +
                    '</tr>';
            });

            $("#detalletalonariosd").append(codigohtml);
            $.fancybox.update();
        }


        function mostrarseriesno(msg) { mensajeerrorpermanente(msg.responseText); }


        //   ----------------------------

        function muestraSeries2() {
            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraSeries2",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar2").removeClass("ocultar");
                    $("#cargar2").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar2").removeClass("mostrar");
                    $("#cargar2").addClass("ocultar");
                },
                success: mostrarseries2,
                error: mostrarseriesno2
            });
        }

        function mostrarseries2(msg) {
            var codigohtml = "";
            $("#detalletalonariosd2").html("");
            codigohtml = '';

            $.each(msg.d, function () {
                codigohtml = codigohtml + '<tr>' +
                        '<td>' + this.serie + '</td>' +
                        '<td>' + this.usuario + '</td>' +
                        '<td>' + this.fechacreacion + '</td>' +
                        '<td>' + this.correlativoinicial + '</td>' +
                        '<td>' + this.correlativofinal + '</td>' +
                    '</tr>';


                document.getElementById('txtcorrelativoinicial2').value = this.correlativoinicial;
                $("#txtcorrelativoinicial2").enable(false);
                document.getElementById('hidcorrelativoinicial2').value = this.correlativoinicial;


                document.getElementById('txtcorrelativofinal2').value = this.correlativofinal;
                $("#txtcorrelativofinal2").enable(false);
                document.getElementById('hidcorrelativofinal2').value = this.correlativofinal;


                $('#dropserie2').append('<option value="' + this.correlativo + '">' + this.serie + '</option>');

                $('#utilizado').append('<input type="number" name="txtutilizado" id="txtutilizado" min="' + this.correlativoinicial + '" max="' + this.correlativofinal + '">');


                //<input type="number" name="txtutilizado" id="txtutilizado" min="1" max="5">





            });

            $("#detalletalonariosd2").append(codigohtml);
            $.fancybox.update();
        }


        function mostrarseriesno2(msg) { mensajeerrorpermanente(msg.responseText); }

        // ---------------------










        function muestraHorarios() {
            var numerotarjeta = document.getElementById('txtnumerotarjeta');
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraHorarios",
                data: '{usuario: "' + usuario + '",numerotarjeta: ' + numerotarjeta.value + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar3").removeClass("ocultar");
                    $("#cargar3").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar3").removeClass("mostrar");
                    $("#cargar3").addClass("ocultar");
                },
                success: mostrarhorarios,
                error: mostrarhorariosno
            });
        }

        function mostrarhorarios(msg) {
            var codigohtml = "";
            $("#tresumenhorarios").html("");
            codigohtml = '';
            var contador = 0;
            $.each(msg.d, function () {
                codigohtml = codigohtml + '<tr>' +
                        '<td>' +
                            '<input type="button" id="' + this.idhorario + '|' + contador + '" value="+" onclick="resumenhorario(this.id);" class="buttonS bGreen"/>' +
                            '<input type="hidden" id="hidhorar' + contador + '" value="' + this.idhora + '" />' +
                            '<input type="hidden" id="hidservicior' + contador + '" value="' + this.idservicio + '" />' +
                            '<input type="hidden" id="hidrutar' + contador + '" value="' + this.idruta + '" />' +
                        '</td>' +
                        '<td id="tdfecha' + contador + '">' + this.fecha + '</td>' +
                        '<td>' + this.hora + '</td>' +
                        '<td>' + this.servicio + '</td>' +
                        '<td>' + this.ruta + '</td>' +
                        '<td>' + this.piloto + '</td>' +
                        '<td>' + this.asistente + '</td>' +
                    '</tr>';
                contador++;
            });

            $("#tresumenhorarios").append(codigohtml);
            $.fancybox.update();
        }

        function mostrarhorariosno(msg) { mensajeerrorpermanente(msg.responseText); }

        function resumenhorario(control) {
            $("#tresumenhorariose").fadeOut('slow');
            var objeto = document.getElementById(control);
            var arreglo = objeto.id.split("|");
            var correlativo = arreglo[1];

            var idhora = document.getElementById('hidhorar' + correlativo).value;
            var idruta = document.getElementById('hidrutar' + correlativo).value;
            var idservicio = document.getElementById('hidservicior' + correlativo).value;
            var fecha = document.getElementById('tdfecha' + correlativo).innerHTML;
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/muestraResumen",
                data: '{usuario: "' + usuario + '",idhorario: ' + arreglo[0] + ',fecha: "' + formatofecha(fecha) + '",idhora: ' + idhora + ',idruta: ' + idruta + ',idservicio: ' + idservicio + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar3").removeClass("ocultar");
                    $("#cargar3").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar3").removeClass("mostrar");
                    $("#cargar3").addClass("ocultar");
                },
                success: muestraresumen,
                error: muestraresumenno
            });

        }

        function formatofecha(fecha) {
            var arreglo = fecha.split("/");
            var dia = arreglo[0];
            var mes = arreglo[1];
            var anio = arreglo[2];
            if (parseInt(dia.length) < 2)
                dia = "0" + dia;
            if (parseInt(mes.length) < 2)
                mes = "0" + mes;
            return anio + mes + dia;
        }

        function muestraresumen(msg) {
            var codigohtml = "";
            $("#tmontosdetalle").html("");
            codigohtml = '';

            $.each(msg.d, function () {
                codigohtml = codigohtml + '<tr>' +
                        '<td>' + this.origen + '</td>' +
                        '<td>' + this.cantidadboletos + '</td>' +
                        '<td>' + this.montoboletos + '</td>' +
                    '</tr>';
                document.getElementById('btnregresar').style.display = '';
            });

            $("#tmontosdetalle").fadeIn("slow");
            $("#tmontos").fadeIn("slow");
            $("#tmontos").append(codigohtml);

            $.fancybox.update();
        }

        function muestraresumenno(msg) { mensajeerrorpermanente(msg.responseText); }

        function cargarSerie(control) {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/cargarSeries",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: cargaseries,
                error: cargaseriesno
            });
        }

        function cargaseries(msg) {
            $('#dropserie').append('<option value="0">Seleccione una serie</option>');

            $.each(msg.d, function () {
                $('#dropserie').append('<option value="' + this.correlativo + '">' + this.serie + '</option>');
            });
        }

        function cargaseriesno(msg) {
            mensajeerrorpermanente(msg.responseText);
        }

        function busquedaserie(serie, ultimocorrelativo) {

            var tarjetanumero = $("#txtnumerotarjeta").val();

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/buscarSerie",
                data: '{serie: "' + serie.value + '",ultimocorrelativo: ' + ultimocorrelativo.value + ',usuario: "' + usuario + '"' + ',tarjetanumero: ' + tarjetanumero + '}',
                //data: '{serie: "' + serie.value + '",ultimocorrelativo: ' + ultimocorrelativo.value + ',usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: buscarserie,
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                error: buscarserieno
            });
        }

        function buscarserie(msg) {
            var codigohtml = "";
            var contador = 0;
            var incremento = 0;
            var final = document.getElementById('txtultimocorrelativo');
            var color = "";
            $("#detalleboletos").html("");

            $.each(msg, function () {
                if (this.correcto == true) {
                    mensajecorrecto(this.mensaje);
                    correlativo = this.correlativo;

                    for (contador = 0; correlativo <= final.value; contador++) {

                        if (contador % 2 == 0) color = "#DEE3F5";
                        else color = "#BCCCD1";

                        codigohtml = '<div class="formRow" style="background:' + color + ';padding-top:0px;"><div class="grid2">' +
                        '<span class="note" style="padding-top:0px;">CORREL</span>' +
                        '<input type="text"  value="' + correlativo + '"  id="txtboleto' + contador + '"  disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid3">' +
                        '   <span class="note" style="padding-top:0px;">ORIGEN</span>' +
                        '   <select id="droporigen' + contador + '" style="width:108px"></select>' +
                        '</div>' +
                        '<div class="grid3">' +
                        '   <span class="note" style="padding-top:0px;">DESTINO</span>' +
                        '   <select id="dropdestino' + contador + '" style="width:108px"></select>' +
                        '</div>' +
                        '<div class="grid2">' +
                        '   <span class="note" style="padding-top:0px;">VALOR</span>' +
                        '   <input type="text" onblur="calculototalfacturacion()" value="" id="txtvalor' + contador + '"/>' +
                        '</div>' +
                        '<div class="grid1">' +
                        '   <span class="note" style="padding-top:0px;">COBRO RUTA</span>' +
                        '   <select id="droptipocobroruta' + contador + '" style="width:75px"></select>' +
                        '</div>' +
                        '</div>'



                        $("#detalleboletos").append(codigohtml);



                        cargadropgenerico(document.getElementById('droporigen' + contador).id, origenes);
                        cargadropgenerico(document.getElementById('dropdestino' + contador).id, origenes);
                        cargadropgenerico(document.getElementById('droptipocobroruta' + contador).id, origenestipocobroruta);

                        correlativo++;

                    }

                    color = "rgba(156, 174, 179, 0.42)";

                    codigohtml = '<div style="background:' + color + ';height: 20px;padding-top:0px;padding: 8px 16px;border-top: 1px solid #fff;border-bottom: 1px solid #ddd;content: "";display: block; clear: both;"><div class="grid2">' +
                        '<span class="note" style="padding-top:0px;"> &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp</span>' +
                        '</div>' +
                                       '<div class="grid4">' +
                        '<span class="note" style="text-align: right;font-weight: bold;">TOTAL A FACTURAR</span>' +
                        '</div>' +
                         '<div class="grid4">' +
                                           '<input type="text" id="txttotalfacturacion"   style="margin-top: 5px;font-size: 15px;text-align: right;font-weight: bold;width: 147px;" disabled="disabled" value="0.00"/>' +
                    '</div>' +
                    '<div class="grid4" style="display:none">' +
                                           '<input type="text" id="txttotalfacturas"   style="margin-top: 5px;" disabled="disabled" value="' + contador + '"/>' +
                    '</div>' +

                    '<div class="grid2">' +
                        '<span class="note" style="padding-top:0px;"> &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp</span>' +
                        '</div>' +
                  '</div>';

                    $("#detalleboletos").append(codigohtml);

                }
                else { mensajeadvertencia(this.mensaje); document.getElementById('dropserieventa').focus(); }
            });


            $.fancybox.update();

        }




        function busquedaseriecarga(serie, ultimocorrelativo) {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/buscarSerie",
                data: '{serie: "' + serie.value + '",ultimocorrelativo: ' + ultimocorrelativo.value + ',usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: buscarseriecarga,
                beforeSend: function () {
                    $("#cargar1").removeClass("ocultar");
                    $("#cargar1").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar1").removeClass("mostrar");
                    $("#cargar1").addClass("ocultar");
                },
                error: buscarserieno
            });
        }

        function buscarseriecarga(msg) {
            var codigohtml = "";
            var contador = 0;
            var incremento = 0;
            var final = document.getElementById('txtultimocorrelativocarga');
            var color = "";
            $("#detallecarga").html("");

            $.each(msg, function () {
                if (this.correcto == true) {
                    mensajecorrecto(this.mensaje);
                    correlativo = this.correlativo;

                    for (contador = 0; correlativo <= final.value; contador++) {

                        if (contador % 2 == 0) color = "#BCCCD1";
                        else color = "#C0C0C0";

                        codigohtml = '<div class="formRow" style="background:' + 'transparent' + ';padding-top:0px;padding-left: 130px;"><div class="grid4" style=" background: ' + color + '; ">' +
                        '<span class="note" style="padding-top:0px;">CORRELATIVO</span>' +
                        '<input type="text"  value="' + correlativo + '"  id="txtfacturacarga' + contador + '"  disabled="disabled"/>' +
                    '</div>' +
                    '<div class="grid4" style=" background:' + color + ' ">' +
                        '<span class="note" style="padding-top:0px;">VALOR CARGA</span>' +
                        '<input type="text" onblur="calculototalfacturacion()" value="" id="txtvalor' + contador + '"/>' +
                    '</div>' +
                    '</div>'


                        $("#detallecarga").append(codigohtml);
                        correlativo++;

                    }

                    color = "rgba(156, 174, 179, 0.42)";

                    codigohtml = '<div style="background:' + color + ';height: 20px;padding-top:0px;padding: 8px 16px;border-top: 1px solid #fff;border-bottom: 1px solid #ddd;content: "";display: block; clear: both;"><div class="grid2">' +
                        '<span class="note" style="padding-top:0px;"> &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp</span>' +
                        '</div>' +
                                       '<div class="grid4">' +
                        '<span class="note" style="text-align: right;font-weight: bold;">TOTAL A FACTURAR</span>' +
                        '</div>' +
                         '<div class="grid4">' +
                                           '<input type="text" id="txttotalfacturacion"   style="margin-top: 5px;font-size: 15px;text-align: right;font-weight: bold;width: 147px;" disabled="disabled" value="0.00"/>' +
                    '</div>' +
                    '<div class="grid4" style="display:none">' +
                                           '<input type="text" id="txttotalfacturas"   style="margin-top: 5px;" disabled="disabled" value="' + contador + '"/>' +
                    '</div>' +

                    '<div class="grid2">' +
                        '<span class="note" style="padding-top:0px;"> &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp &nbsp</span>' +
                        '</div>' +
                  '</div>';


                    $("#detallecarga").append(codigohtml);

                }
                else { mensajeadvertencia(this.mensaje); document.getElementById('dropseriecarga').focus(); }
            });


            $.fancybox.update();

        }



        function calculototalfacturacion() {

            var i = 0;
            var totalfacturas = 0;
            var totalfacturaactual = 0;
            var totalfacturacion = 0;
            document.getElementById("txttotalfacturacion").value = 0;
            totalfacturas = $("#txttotalfacturas").val();


            while (i <= totalfacturas - 1) {


                totalfacturaactual = $("#txtvalor" + i).val();
                if (totalfacturaactual == "") { totalfacturaactual = "0.00"; }

                totalfacturacion = parseFloat(totalfacturacion).toFixed(2) * 1 + parseFloat(totalfacturaactual).toFixed(2) * 1;
                totalfacturacion = parseFloat(totalfacturacion).toFixed(2);
                i = i + 1;
            }

            document.getElementById("txttotalfacturacion").value = totalfacturacion





        }



        function cargadropgenerico(drop, arreglo) {
            /*FUNCION PARA DETERMINAR EL NOMBRE DE LAS COLUMNAS*/

            $.each(arreglo, function (key, row) {
                var contador = 0;
                var valor = "";
                var texto = "";
                $.each(row, function (key, fieldValue) {
                    if (contador == 1) valor = fieldValue;
                    else if (contador == 2) {
                        texto = fieldValue;
                        $('#' + drop).append('<option value="' + valor + '">' + texto + '</option>');
                    }
                    contador++;
                    if (contador == 3) contador = 0;
                });

            });

        }


        function buscarserieno(msg) {
            mensajeerrorpermanente(msg.responseText);
        }

        function modalboletos(control) {
            var valor = document.getElementById(control).id;
            var correlativo = valor.substring(17, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;
            muestraSeriesDrop();
            muestraBoletosvendidos(correlativo);
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");
            $("#boletosguardados").html("");
            $("#dropserieventa").html("");
            $.fancybox.open("#boletos");

        }



        function modalboletosagencia(control) {

            var valor = document.getElementById(control).id;
            var correlativo = valor.substring(24, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;
            muestraBoletosvendidos(correlativo);

            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");
            $("#boletosguardados").html("");
            $("#dropserieventa").html("");
            $.fancybox.open("#boletosagencia");

        }



        function modalcarga(control) {

            control = $("#numerocarga").html();
            var valor = document.getElementById(control).id;
            var correlativo = valor.substring(15, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;
            muestraSeriesDrop();
            muestraCargaVendida(correlativo);
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");
            $("#boletosguardados").html("");
            $("#dropserieventa").html("");
            $("#dropseriecarga").html("");
            $.fancybox.open("#carga");
        }

        function modalcargaagencia(control) {

            control = $("#numerocarga").html();
            var valor = document.getElementById(control).id;
            var correlativo = valor.substring(15, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;


            muestraCargaAgenciaVendida(correlativo);
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");
            $("#boletosguardados").html("");
            $("#dropserieventa").html("");
            $("#dropseriecarga").html("");
            $.fancybox.open("#cargaagencia");
        }

        function modalseleccioncarga(control) {
            var valor = document.getElementById(control).id;
            var correlativo = valor.substring(15, valor.length);
            document.getElementById('hcorrelativo').value = correlativo;
            muestraSeriesDrop();
            muestraCargaVendida(correlativo);
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");
            $("#boletosguardados").html("");
            $("#dropserieventa").html("");
            $("#numerocarga").html(control);
            $.fancybox.open("#seleccioncarga");

        }


        function abrimodal(control) {
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");

            $.fancybox.open("#" + control);
        }

        function mostrarmodal(control) {
            $.fancybox.open("#boletos");
        }
        function cerrarmodal(control) {
            $.fancybox.close("#" + control)
        }



        function abrimodal2(control) {
            $("#detalleboletos").html("");
            $("#detalletalonariosd").html("");

            $.fancybox.open("#" + control);
        }

        function mostrarmodal2(control) {
            $.fancybox.open("#boletos");
        }
        function cerrarmodal2(control) {
            $.fancybox.close("#" + control)
        }







        function agregardetalle(msg) {
            var mensaje = "";
            var codigohtml = "";
            var contador = $("#detalle .formRow").length;
            contador++;
            var color = "";
            if (contador % 2 == 0) color = "#DEE3F5";
            else color = "#BCCCD1";

            $.each(msg, function () {
                mensaje = this.fechacreacion;
                if (mensaje.substring(0, 5) == "ERROR") {
                    mensajeerrorpermanente(mensaje);
                } else {
                    mensajecorrecto("Detalle agregado correctamente");
                    /*GENERAMOS CODIGO HTML*/
                    codigohtml = codigohtml + '<div class="formRow" id="divdetalle' + contador + '" style="background:' + color + ';">' +
					    '<div class="grid2">' +
                            '<input type="hidden" id="hidruta' + contador + '" value="' + this.idruta + '" />' +
                            '<input type="hidden" id="hidhora' + contador + '" value="' + this.idhora + '" />' +
                            '<input type="hidden" id="hidservicio' + contador + '" value="' + this.idservicio + '" />' +
                            '<input type="hidden" id="hidtarjetadetalle' + contador + '" value="' + this.idtarjetadetalle + '" />' +
                            '<input id="txtfechahorario' + contador + '" type="text" value="' + this.fechahorario + '" name="regular" disabled="disabled" style="display:none;"/>' +
                            '<span class="note">FECHA VIAJE</span>' +
                            '<input id="txtfechacreacion' + contador + '" type="text" value="' + this.fechacreacion + '" name="regular" disabled="disabled" />' +
                        '</div>' +
                        '<div class="grid2">' +
                            '<span class="note">EMPRESA</span>' +
                            '<input id="txtempresa' + contador + '" type="text" value="' + this.empresa + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">AGENCIA</span>' +
                            '<input id="txtagencia' + contador + '" type="text" value="' + this.agencia + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">HORA</span>' +
                            '<input id="txthora' + contador + '" type="text" value="' + this.hora + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">RUTA</span>' +
                            '<input id="txtruta' + contador + '" type="text" value="' + this.ruta + '" title="' + this.ruta + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">SERVICIO</span>' +
                            '<input id="txtservicio' + contador + '" type="text" value="' + this.servicio + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid2" style="margin-left:0px;">' +
                            '<span class="note">USUARIO</span>' +
                            '<input id="txtusuario' + contador + '" type="text" value="' + this.idusuario + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">PILOTO</span>' +
                            '<input id="txtpiloto' + contador + '" type="text" value="' + this.piloto + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">ASISTENTE</span>' +
                            '<input id="txtasistente' + contador + '" type="text" value="' + this.asistente + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style="display:none">' +
                            '<span class="note">VALE</span>' +
                            '<input id="txtnumerovale' + contador + '" type="text" value="' + 0 + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style="display:none">' +
                            '<span class="note">CARGA</span>' +
                            '<input id="txtcarga' + contador + '" type="text" value="' + this.carga + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid1" style="display:none">' +
                            '<span class="note">EXCURSION</span>' +
                            '<input id="txtexcursion' + contador + '" type="text" value="' + this.excursion + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid1">' +
                            '<span class="note">VIATICOS PIL.</span>' +
                            '<input id="txtviaticospiloto' + contador + '" type="text" value="' + this.viaticospiloto + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid1">' +
                            '<span class="note">VIATICOS ASIS.</span>' +
                            '<input id="txtviaticosasistente' + contador + '" type="text" value="' + this.viaticosasistente + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style=" width: 14%; margin: 4px 0px 0px 20px;">' +
                            '<input type="button" value="R" onclick="modalboletos(this.id);" class="buttonS bRed" title="Registrar Boletos Vendidos En Ruta" id="btnagregarboletos' + contador + '" style="margin-top:20px;width:4px;padding-left: 7px;"/>' +
                            '<input type="button" value="A" onclick="modalboletosagencia(this.id);" class="buttonS bGold" title="Registrar Boletos Vendidos En Agencia" id="btnagregarboletosagencia' + contador + '" style="margin-top:20px;;width:4px;padding-left: 7px;"/>' +
                            '<input type="button" value="C" onclick="modalseleccioncarga(this.id);" class="buttonS bLightBlue" title="Registrar Carga" id="btnagregarcarga' + contador + '" style="margin-top:20px;;width:4px;padding-left: 7px;"/>' +
                        '</div>' +
                    '</div>';
                    /*CODIGO HTML*/

                    var droporigen = document.getElementById('droporigen');
                    var drophorario = document.getElementById('drophorario');
                    var carga = document.getElementById('txtcarga');
                    var excursion = document.getElementById('txtexcursion');
                    var viaticospiloto = document.getElementById('txtviaticospiloto');
                    var viaticosasistente = document.getElementById('txtviaticosasistente');
                    var numerovale = document.getElementById('txtidvale');

                    droporigen.focus();
                    //$("#drophorario").html("");
                    carga.value = "";
                    excursion.value = "";
                    viaticospiloto.value = "";
                    viaticosasistente.value = "";
                    numerovale.value = "";
                }
            });


            $("#detalle").append(codigohtml);
        }

        function agregardetalleno(msg) {

            mensajeerrorpermanente(msg.responseText);
        }

        function cargarTurno(origen) {
            var origen = $('#droporigen').val();
            var fecha = document.getElementById('txtfechaviaje');
            var vehiculo = $('#DropVehiculo').val();


            if (origen != "" && fecha.value != "") {
                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/TurnoSugerido",
                    data: '{Vehiculo: "' + vehiculo + '",Fecha: "' + fecha.value + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: resultadohorarios,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: erroreshorarios
                });
            }
            else {
                mensajeadvertencia("No ha seleccionado una fecha valida o un origen valido");
                fecha.focus();
            }
        }

        function resultadohorarios(msg) {
            $("#drophorario").html("");
            var seleccionar = false;
            $.each(msg.d, function (index) {
                $('#drophorario').append('<option value="' + this.datos + '">' + this.hora + '</option>');
            });

            $("#dhorariosasignados").html('');

            $("#drophorario option").each(function () {

                $("#dhorariosasignados").append('<tr><td>' + $(this).text() + '</td></tr>');

            });
        }

        function erroreshorarios(msg) { mensajeerrorpermanente(msg.responseText); }

        function cargarOrigen(control) {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/DestinosBoletos",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    origenes = msg.d;
                    $.each(msg.d, function () {
                        $('#' + control).append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
                    });
                },
                error: function (msg) { mensajeerrorpermanente("Error " + msg.responseText); }
            });
        }


        function cargarTipoCobroRuta(control) {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/TipoCobroRuta",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    origenestipocobroruta = msg.d;
                    $.each(msg.d, function () {
                        $('#' + control).append('<option value="' + this.idtipocobroruta + '">' + this.nombre + '</option>');
                    });
                },
                error: function (msg) { mensajeerrorpermanente("Error " + msg.responseText); }
            });
        }



        function crearTarjeta() {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/crearTarjeta",
                data: '{usuario: "' + usuario + '"}',
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar").removeClass("mostrar");
                    $("#cargar").addClass("ocultar");
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: crear,
                error: crearno
            });
        }

        function crear(msg) {

            $.each(msg, function () {
                if (this.encontro == true) {
                    mensajecorrecto(this.mensaje);
                    $("#filaencabezado").slideDown('slow');
                    $("#pie").slideDown('slow');

                    if (this.idtarjetaestado == 1) $("#filaingresodetalle").slideDown('slow');

                    document.getElementById('txtnumerotarjeta').disabled = true;
                    document.getElementById('btnbuscar').disabled = true;
                    document.getElementById('txtagenciae').value = this.agencia;
                    document.getElementById('txtusuarioe').value = this.idusuario;
                    document.getElementById('txtempresae').value = this.empresa;
                    document.getElementById('txttarjetaestado').value = this.tarjetaestado;
                    document.getElementById('txtfechacreacione').value = this.fechacreacion;
                    document.getElementById('txtidtarjeta').value = this.idtarjeta;
                    document.getElementById('txtnumerotarjeta').value = this.numerotarjeta;
                    //setTimeout('document.getElementById("txtfechaviaje").focus();',500);
                }
            });
        }

        function crearno(msg) { mensajeerrorpermanente(msg.responseText); }

        function buscarTarjeta(numerotarjeta) {
            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/buscarTarjeta",
                data: '{numerotarjeta: ' + numerotarjeta + ',usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar").removeClass("mostrar");
                    $("#cargar").addClass("ocultar");
                },
                success: buscar,
                error: buscarno
            });
        }

        function buscar(msg) {

            $.each(msg, function () {
                if (this.encontro == true) {
                    mensajecorrecto(this.mensaje);
                    $("#filaencabezado").slideDown('slow');
                    $("#pie").slideDown('slow');
                    if (this.idtarjetaestado == 1) {
                        $("#filaingresodetalle").slideDown('slow');
                        document.getElementById('btngenerarseries').style.display = '';
                        document.getElementById('btnguardarboletos').style.display = '';
                        document.getElementById('btnasignarserie').style.display = '';
                        document.getElementById('btncerrartarjeta').style.display = '';
                        document.getElementById('btnagregarhorario').style.display = '';
                        document.getElementById('btnguardarboletosagencia').style.display = '';
                        document.getElementById('btnliberarhorario').style.display = '';

                        document.getElementById('btnagregartalonario').style.display = '';
                        document.getElementById('btnentregartalonario').style.display = '';

                    }
                    else if (this.idtarjetaestado == 2) {
                        document.getElementById('btngenerarseries').style.display = 'none';
                        document.getElementById('btnguardarboletos').style.display = 'none';
                        document.getElementById('btnasignarserie').style.display = 'none';
                        document.getElementById('btncerrartarjeta').style.display = 'none';
                        document.getElementById('btnagregarhorario').style.display = 'none';
                        document.getElementById('btnguardarboletosagencia').style.display = 'none';
                        document.getElementById('btnliberarhorario').style.display = 'none';

                        document.getElementById('btnagregartalonario').style.display = 'none';
                        document.getElementById('btnentregartalonario').style.display = 'none';
                    }

                    $('#DropVehiculo').html('');
                    $('#DropVehiculo').append('<option value=' + this.vehiculo + '>' + this.vehiculo + '</option>');

                    document.getElementById('DropVehiculo').disabled = true;
                    document.getElementById('txtfechaviaje').disabled = true;
                    document.getElementById('txtnumerotarjeta').disabled = true;

                    document.getElementById('btnasignarhorario').value = "Actualizar Horarios";


                    document.getElementById('titulo').innerHTML = "TARJETA DE RUTA DEL VEHICULO: " + this.vehiculo;

                    $('#titulo').css('color', 'rgb(82, 119, 123');
                    document.getElementById('btnbuscar').disabled = true;
                    document.getElementById('txtagenciae').value = this.agencia;
                    document.getElementById('txtusuarioe').value = this.idusuario;
                    document.getElementById('txtempresae').value = this.empresa;
                    document.getElementById('txttarjetaestado').value = this.tarjetaestado;
                    document.getElementById('txtfechacreacione').value = this.fechacreacion;
                    document.getElementById('txtidtarjeta').value = this.idtarjeta;
                    document.getElementById('txtnumerotarjeta').value = this.numerotarjeta;
                    //setTimeout('document.getElementById("txtfechaviaje").focus();', 1000);
                }

                else {
                    mensajeerror(this.mensaje);
                }


            });

            var contador = 1;
            var codigohtml = "";
            var color = "";

            $('#detalle').html("");

            $.each(msg.d.Detalle, function () {
                if (contador % 2 == 0) color = "#DEE3F5";
                else color = "#BCCCD1";

                codigohtml = codigohtml + '<div class="formRow" id="divdetalle' + contador + '" style="background:' + color + ';">' +
					    '<div class="grid2">' +
                            '<input type="hidden" id="hidruta' + contador + '" value="' + this.idruta + '" />' +
                            '<input type="hidden" id="hidhora' + contador + '" value="' + this.idhora + '" />' +
                            '<input type="hidden" id="hidservicio' + contador + '" value="' + this.idservicio + '" />' +
                            '<input type="hidden" id="hidtarjetadetalle' + contador + '" value="' + this.idtarjetadetalle + '" />' +
                            '<input id="txtfechahorario' + contador + '" type="text" value="' + this.fechahorario + '" name="regular" disabled="disabled" style="display:none;"/>' +
                            '<span class="note">FECHA VIAJE</span>' +
                            '<input id="txtfechacreacion' + contador + '" type="text" value="' + this.fechacreacion + '" name="regular" disabled="disabled" />' +
                        '</div>' +
                        '<div class="grid2">' +
                            '<span class="note">EMPRESA</span>' +
                            '<input id="txtempresa' + contador + '" type="text" value="' + this.empresa + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">AGENCIA</span>' +
                            '<input id="txtagencia' + contador + '" type="text" value="' + this.agencia + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">HORA</span>' +
                            '<input id="txthora' + contador + '" type="text" value="' + this.hora + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">RUTA</span>' +
                            '<input id="txtruta' + contador + '" type="text" value="' + this.ruta + '" title="' + this.ruta + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">SERVICIO</span>' +
                            '<input id="txtservicio' + contador + '" type="text" value="' + this.servicio + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid2" style="margin-left:0px;">' +
                            '<span class="note">USUARIO</span>' +
                            '<input id="txtusuario' + contador + '" type="text" value="' + this.idusuario + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">PILOTO</span>' +
                            '<input id="txtpiloto' + contador + '" type="text" value="' + this.piloto + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid2">' +
                            '<span class="note">ASISTENTE</span>' +
                            '<input id="txtasistente' + contador + '" type="text" value="' + this.asistente + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style="display:none">' +
                            '<span class="note">VALE</span>' +
                            '<input id="txtnumerovale' + contador + '" type="text" value="' + this.numerovale + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style="display:none">' +
                            '<span class="note">CARGA</span>' +
                            '<input id="txtcarga' + contador + '" type="text" value="' + this.carga + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid1" style="display:none">' +
                            '<span class="note">EXCURSION</span>' +
                            '<input id="txtexcursion' + contador + '" type="text" value="' + this.excursion + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid2">' +
                            '<span class="note">VIATICOS PIL.</span>' +
                            '<input id="txtviaticospiloto' + contador + '" type="text" value="' + this.viaticospiloto + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
                        '<div class="grid2">' +
                            '<span class="note">VIATICOS ASIS.</span>' +
                            '<input id="txtviaticosasistente' + contador + '" type="text" value="' + this.viaticosasistente + '" name="regular" disabled="disabled"/>' +
                        '</div>' +
						'<div class="grid1" style=" width: 14%; margin: 4px 0px 0px 20px;">' +
                             '<input type="button" value="R" onclick="modalboletos(this.id);" class="buttonS bRed" title="Registrar Boletos Vendidos En Ruta" id="btnagregarboletos' + contador + '" style="margin-top:20px;width:4px;padding-left: 7px;"/>' +
                            '<input type="button" value="A" onclick="modalboletosagencia(this.id);" class="buttonS bGold" title="Registrar Boletos Vendidos En Agencia" id="btnagregarboletosagencia' + contador + '" style="margin-top:20px;;width:4px;padding-left: 7px;"/>' +
                            '<input type="button" value="C" onclick="modalseleccioncarga(this.id);" class="buttonS bLightBlue" title="Registrar Carga" id="btnagregarcarga' + contador + '" style="margin-top:20px;;width:4px;padding-left: 7px;"/>' +
                         '</div>' +
                    '</div>';
                contador++;

            });
            $("#detalle").append(codigohtml);
            /*  */
            cargarHorarios();

        }

        function buscarno(msg) { mensajeerrorpermanente(msg.responseText); }

        function mostrarbusqueda() {
            document.getElementById('titulo').innerHTML = 'Busqueda de Tarjeta de ruta';
            document.getElementById('filaencabezado').style.display = "none";
            document.getElementById('btnbuscar').value = "Buscar";
            mensajecorrecto("Busqueda de Tarjeta de ruta");
            $("#btncancelar").click();
        }

        function mostrarcreacion() {
            document.getElementById('titulo').innerHTML = 'Creacion Tarjeta de ruta';
            document.getElementById('filaencabezado').style.display = "none";
            mensajecorrecto("Creacion de Tarjeta de ruta");
            $("#btncancelar").click();
            var tarjeta = document.getElementById('txtnumerotarjeta');
            tarjeta.disabled = true;
            tarjeta = document.getElementById('btnbuscar').focus();
            document.getElementById('btnbuscar').value = "Crear";
            document.getElementById('DropVehiculo').disabled = "";
            document.getElementById('txtfechaviaje').disabled = "";


        }


        function tarjetadetalladapdf(tarjeta) {

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/ResumenTarjetaPdf",
                data: '{numerotarjeta: ' + tarjeta + ',usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resumenboletospdf,
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar").removeClass("mostrar");
                    $("#cargar").addClass("ocultar");
                },
                error: resumenboletospdfno
            });
        }



        function resumenboletospdf(msg) {
            var respuesta = msg.d;

            if (respuesta.substring(0, 5) == "ERROR") {
                mensajeerror(respuesta);
            }
            else {
                abremodal(respuesta);
            }
        }

        function resumenboletospdfno(msg) { mensajeerrorpermanente(msg.responseText); }

        function replaceAll(text, busca, reemplaza) {
            while (text.toString().indexOf(busca) != -1)
                text = text.toString().replace(busca, reemplaza);
            return text;
        }


        function mostrarresumentarjetas() {

            $("#divbusqueda").fadeOut("slow");
            $("#divbusqueda").css('margin', '50px -80px');
            $("#sidebar").remove();


            $("#divbusqueda").fadeIn("slow");
            $("#btncancelar").click();

            var codigohtml = "";

            $("#divbusqueda").html('');
            $(".tipsy-inner").remove();
            $(".tipsy-arrow").remove();
            $("#divbusqueda").css('width', '113%');
            $("#divbusqueda").css('margin', '50px -15%');

            //   codigohtml = "<div class='whead'><h6 id='titulo'> RESUMEN DE TARJETAS </h6></div>" +
            //  "<div class='formrow'><div class='grid2' style=' margin: 10px -30px 10px 25px;'> Fecha Inicial  <input id='txtfechainicialreporte' class='textboxshadow' style='text-align: right;width:130px;height: 27px;border: solid 1px #BEBEBE;' type='text' name='regular' /> </div> <div class='grid2' style=' margin: 10px -30px 10px 0px;'>Fecha Final  <input id='txtfechafinalreporte' type='text' style=' text-align: right;height: 27px;width:130px;border: solid 1px #BEBEBE;' class='textboxshadow' name='regular' /> </div>  " +
            //  "<div class='grid2' style=' margin: 10px -5px 10px 0px;'> Agencia    <select id='DropAgenciaReporte' style='width:80%;'></select></div>" +
            //   "<div class='grid2' style=' margin: 10px -5px 10px 0px;'> Vehiculo  &nbsp;  <select id='DropVehiculoReporte' style='width:80%;'></select></div>" +
            //   "<div class='grid2' style=' margin: 10px -5px 10px 0px;'> Piloto  &nbsp;&nbsp;&nbsp;<select id='DropPilotoReporte' style='width:80%;'></select></div>" +
            //   "<div class='grid2' style=' margin: 10px -5px 10px 0px;'> Servicio  &nbsp;&nbsp;&nbsp;<select id='DropServicioReporte' style='width:80%;'></select></div>" +
            //   " <div class='grid2' style=' margin: 10px -5px 10px 0px;'><p></p><input type='button' value='Generar Reporte' class='buttonS bLightBlue' id='btnresumentarjetas' style=' margin: 10px 10px 10px 0px; '></div></div>" +
            //   "<div id='cargar' class='ocultar' style='float:right;margin-top: -43px;margin-right: -300px;font-weight: bold;'><img src='../images/elements/loaders/4s.gif' alt='Cargando'></div>" +
            //   " <script>  $(function () { $('#txtfechainicialreporte').datepicker({numberOfMonths: 1,dateFormat: 'yy/mm/dd',firstDay: 0});})<" + "/script>" +
            //   " <script>  $(function () { $('#txtfechafinalreporte').datepicker({numberOfMonths: 1,dateFormat: 'yy/mm/dd',firstDay: 0});})<" + "/script>"


            codigohtml = "<div class='whead'><h6 id='titulo'> RESUMEN DE TARJETAS </h6></div>" +
            "<div class='formrow'><div class='grid2' style=' margin: 10px -40px 10px 25px;'> Fecha Inicial  <input id='txtfechainicialreporte' class='textboxshadow' style='text-align: right;width:130px;height: 27px;border: solid 1px #BEBEBE;' type='text' name='regular' /> </div> " +
            "<div class='grid2' style=' margin: 10px -40px 10px 0px;'>Fecha Final  <input id='txtfechafinalreporte' type='text' style=' text-align: right;height: 27px;width:130px;border: solid 1px #BEBEBE;' class='textboxshadow' name='regular' /> </div>  " +
            "<div class='grid2' style=' margin: 10px -30px 10px 0px;'> Agencia    <select id='DropAgenciaReporte' style='width:80%;'></select></div>" +
            "<div class='grid2' style=' margin: 10px -30px 10px 0px;'> Vehiculo  &nbsp;  <select id='DropVehiculoReporte' style='width:80%;'></select></div>" +
            "<div class='grid2' style=' margin: 10px -30px 10px 0px;'> Piloto  &nbsp;&nbsp;<select id='DropPilotoReporte' style='width:80%;'></select></div>" +
            "<div class='grid2' style=' margin: 10px -30px 10px 0px;'> Servicio  &nbsp;&nbsp;<select id='DropServicioReporte' style='width:80%;'></select></div>" +
            " <div class='grid2' style=' margin: 10px -30px 10px 0px;'><p></p><input type='button' value='Generar Reporte' class='buttonS bLightBlue' id='btnresumentarjetas' style=' margin: 10px 10px 10px 0px; '></div>" +
            " <div class='grid2' style=' margin: 10px -40px 10px 0px;'><p></p><input type='button' value='Exportar' class='buttonS bLightBlue' id='btnExport' OnClick = 'ExportToExcel' style=' margin: 10px 10px 10px 0px; '></div></div>" +
            "<div id='cargar' class='ocultar' style='float:right;margin-top: -43px;margin-right: -300px;font-weight: bold;'><img src='../images/elements/loaders/4s.gif' alt='Cargando'></div>" +
            " <script>  $(function () { $('#txtfechainicialreporte').datepicker({numberOfMonths: 1,dateFormat: 'yy/mm/dd',firstDay: 0});})<" + "/script>" +
            " <script>  $(function () { $('#txtfechafinalreporte').datepicker({numberOfMonths: 1,dateFormat: 'yy/mm/dd',firstDay: 0});})<" + "/script>"

            $("#divbusqueda").append(codigohtml);
            $('#txtfechainicialreporte').datepicker('setDate', 'today');
            $('#txtfechafinalreporte').datepicker('setDate', 'today');


            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/CargarVehiculo",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    Vehiculos = msg.d;

                    $('#DropVehiculoReporte').html('');
                    $('#DropVehiculoReporte').append('<option value="%">TODOS</option>');

                    $.each(msg.d, function () {
                        $('#DropVehiculoReporte').append('<option value="' + this.idvehiculo + '">' + this.nombrevehiculo + '</option>');
                    });
                },
                error: function (msg) { alert("Error " + msg.responseText); }
            });

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/CargarVehiculo",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    Vehiculos = msg.d;

                    $('#DropVehiculoReporte').html('');
                    $('#DropVehiculoReporte').append('<option value="%">TODOS</option>');

                    $.each(msg.d, function () {
                        $('#DropVehiculoReporte').append('<option value="' + this.idvehiculo + '">' + this.nombrevehiculo + '</option>');
                    });
                },
                error: function (msg) { alert("Error " + msg.responseText); }
            });


            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/CargarPilotos",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    Pilotos = msg.d;

                    $('#DropPilotoReporte').html('');
                    $('#DropPilotoReporte').append('<option value="%">TODOS</option>');

                    $.each(msg.d, function () {
                        $('#DropPilotoReporte').append('<option value="' + this.idpiloto + '">' + this.nombrepiloto + '</option>');
                    });
                },
                error: function (msg) { alert("Error " + msg.responseText); }
            });

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/CargarAgencias",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    Agencias = msg.d;

                    $('#DropAgenciaReporte').html('');
                    $('#DropAgenciaReporte').append('<option value="%">TODAS</option>');

                    $.each(msg.d, function () {
                        $('#DropAgenciaReporte').append('<option value="' + this.idagencia + '">' + this.nombreagencia + '</option>');
                    });
                },
                error: function (msg) { alert("Error " + msg.responseText); }
            });

            $.ajax({
                type: "POST",
                url: "WStarjetaruta.asmx/CargarServicios",
                data: '{usuario: "' + usuario + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    Servicio = msg.d;

                    $('#DropServicioReporte').html('');
                    $('#DropServicioReporte').append('<option value="%">TODOS</option>');

                    $.each(msg.d, function () {
                        $('#DropServicioReporte').append('<option value="' + this.idservicio + '">' + this.nombreservicio + '</option>');
                    });
                },
                error: function (msg) { alert("Error " + msg.responseText); }
            });


            $("#btnExport").click(function () {

                //alert("mensaje crea el reporte");
                var fechainicial = "";
                var fechafinal = "";
                var agencia = "";
                var servicio = "";
                var vehiculo = "";
                var piloto = "";

                fechainicial = document.getElementById('txtfechainicialreporte').value;
                fechafinal = document.getElementById('txtfechafinalreporte').value;
                vehiculo = document.getElementById('DropVehiculoReporte').value;
                piloto = document.getElementById('DropPilotoReporte').value;
                agencia = document.getElementById('DropAgenciaReporte').value;
                servicio = document.getElementById('DropServicioReporte').value;

                //                     url: "WStarjetaruta.asmx/ImprimeResumen",

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/ExcelImprimeResumen",
                    data: '{FechaInicial: "' + fechainicial + '",FechaFinal:"' + fechafinal + '",Agencia:"' + agencia + '",Servicio:"' + servicio + '",Vehiculo:"' + vehiculo + '",Piloto:"' + piloto + '",Usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) { alert("Listo " + msg.responseText); },
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: function (msg) { alert("Error " + msg.responseText); }
                });

            });


            $("#btnresumentarjetas").click(function () {

                $('#btnresumentarjetas').prop('disabled', true);


                var fechainicial = "";
                var fechafinal = "";
                var agencia = "";
                var servicio = "";
                var vehiculo = "";
                var piloto = "";




                $("#titulo").html(' RESUMEN DE TARJETAS  <a href="#" onclick="mostrarresumentarjetas();" style="text-align:right;margin-left: 875px;"><img src="../images/icons/usual/icon-arrowleft.png" style="width: 20px;"> REGRESAR </a>');

                fechainicial = document.getElementById('txtfechainicialreporte').value;
                fechafinal = document.getElementById('txtfechafinalreporte').value;
                vehiculo = document.getElementById('DropVehiculoReporte').value;
                piloto = document.getElementById('DropPilotoReporte').value;
                agencia = document.getElementById('DropAgenciaReporte').value;
                servicio = document.getElementById('DropServicioReporte').value;


                // EMPRESA

                //alert(                     '{FechaInicial: "' + fechainicial + '",FechaFinal:"' + fechafinal + '",Agencia:"' + agencia + '",Servicio:"' + servicio + '",Vehiculo:"' + vehiculo + '",Piloto:"' + piloto + '",Usuario: "' + usuario + '"}';

                $.ajax({
                    type: "POST",
                    url: "WStarjetaruta.asmx/ResumenTarjetas",
                    data: '{FechaInicial: "' + fechainicial + '",FechaFinal:"' + fechafinal + '",Agencia:"' + agencia + '",Servicio:"' + servicio + '",Vehiculo:"' + vehiculo + '",Piloto:"' + piloto + '",Usuario: "' + usuario + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: resumenboletos,
                    beforeSend: function () {
                        $("#cargar").removeClass("ocultar");
                        $("#cargar").addClass("mostrar");
                    },
                    complete: function () {
                        $("#cargar").removeClass("mostrar");
                        $("#cargar").addClass("ocultar");
                    },
                    error: resumenboletosno
                });

                function resumenboletos(msg) {
                    $("#divbusqueda").fadeOut("slow");
                    $(".formRow").html('');
                    $(".grid2").remove();


                    datos = msg.d;
                    var codigohtmlresult = "";
                    var codigohtmlfinal = "";


                    var Encabezado = "<table class='tablereporte'><tr class='tablereporte'><th class='tablereporteth'>FECHA VIAJE</th><th class='tablereporteth'># TARJETA</th><th class='tablereporteth'>BUS</th><th class='tablereporteth'>HORARIOS</th><th class='tablereporteth'>SERVICIO</th><th class='tablereporteth'>RUTAS</th><th class='tablereporteth'>PILOTOS</th><th class='tablereporteth'>ASISTENTE</th><th class='tablereporteth'>AGENCIA</th><th class='tablereporteth'>USUARIO</th><th class='tablereporteth'>TOTAL B. RUTA</th><th class='tablereporteth'>TOTAL B. AGENCIA</th><th class='tablereporteth'>TOTAL CARGA</th><th class='tablereporteth'>TOTAL TARJETA</th></tr> ";
                    var colorOff = "background: rgb(241, 241, 241) ; border: 1px solid #cdcdcd; border-radius: 3px; -webkit-border-radius: 3px; -moz-border-radius: 3px; box-shadow: 0 2px 2px -2px #ccc; ";
                    var colorOn = "background: white; border: 1px solid #cdcdcd; border-radius: 3px; -webkit-border-radius: 3px; -moz-border-radius: 3px; box-shadow: 0 2px 2px -2px #ccc; ";
                    var estiloTotales = "background: rgb(241, 241, 241) ;border: 1px solid #cdcdcd; border-radius: 3px; -webkit-border-radius: 3px; -moz-border-radius: 3px; box-shadow: 0 2px 2px -2px #ccc; ;font-size: 1.5em;;font-size: 1.5em;font-size: 1.5em;;font-size: 1.5em; ;border-top: 3px double;";
                    var totales = " <th class='tablereporteth' colspan='3' style='" + estiloTotales + "'> TOTALES</th>";
                    var estiloPdf = ";font-size: 15px;text-decoration: underline;font-weight: bold;'><a href='#' title='Hacer clic aqui para ver un resumen de la tarjeta' onclick=tarjetadetalladapdf('"
                    var estiloMoneda = ";font-size: 1.4em;";
                    var tr = "<tr class='tablereporte'>";
                    var td = "<td class='tablereportetd' style='";
                    var dbl = " ;border-top: 3px double;"

                    $.each(msg.d, function () {

                        this.fila = replaceAll(this.fila, "<table>", Encabezado);
                        this.fila = replaceAll(this.fila, "#tr", tr);
                        this.fila = replaceAll(this.fila, "#td", td);
                        this.fila = replaceAll(this.fila, "#dbl", dbl);
                        this.fila = replaceAll(this.fila, "#pdf", estiloPdf);
                        this.fila = replaceAll(this.fila, "#totales", totales);
                        this.fila = replaceAll(this.fila, "#on", colorOn);
                        this.fila = replaceAll(this.fila, "#off", colorOff);
                        this.fila = replaceAll(this.fila, "#Q", estiloMoneda);
                        codigohtmlresult += this.fila;


                    });






                    $("#divbusqueda").append(codigohtmlresult);
                    $("#divbusqueda").fadeIn("slow");


                }

                function resumenboletosno(msg) { mensajeerrorpermanente(msg.responseText); }






            });
        }

    </script>
    <script src="js/jquery.PrintArea.js" type="text/JavaScript" language="javascript"></script>
    <script>

        $(document).ready(function () {
            $("#imprime").click(function () {
                $("div#detalletalonarios").printArea();
            })


        });




    </script>
    <script>


        $(document).ready(function () {
            $("#imprime2").click(function () {
                $("div#detalletalonarios2").printArea();
            })
        });


    </script>
</head>
<body>
    <%--<input type="text" id="texto1" value="Primer textbox">
<input type="text" id="texto2" value="Segundo textbox">
    --%>
    <!-- Top line begins -->
    <%--<img src="<% =Session("UrlImagen")%>" style="width: 90px" alt="" />--%>
    <!-- Top line ends -->
    <!-- Sidebar begins -->
    <%--    <div id="sidebar" style="width: 200px">
    --%>
    <!-- Main nav -->
    <%-- <div class="mainNav">--%>
    <%--            <div class="user">
                <a title="" class="leftUserDrop" style="margin-top: 50px;"><span id="usuario">Usuario</span>
                    <ul class="leftUser">
    --%>
    <%--                    <!--USUARIO MENU-->
                    <!-- #INCLUDE FILE="menuusuario.html" -->
    --%>
    <%--
                    </ul>
            </div>--%>
    <!-- Responsive nav -->
    <%--            <div class="altNav">
    --%>
    <div class="userSearch">
        <form action="">
        <input type="text" placeholder="search..." name="userSearch" />
        <input type="submit" value="" />
        </form>
    </div>
    <!-- User nav -->
    <%--                <ul class="userNav">
    --%>
    <%--                    <li><a href="#" title="" class="profile"></a></li>
                    <li><a href="#" title="" class="messages"></a></li>
                    <li><a href="#" title="" class="settings"></a></li>
    --%>
    <%--                    <li><a href="cerrarsesion.aspx" title="" class="logout"></a></li>
    --%>
    <%--  </ul>--%>
    <%--            </div>
    --%>
    <%--            <!-- Main nav -->
            <!-- #INCLUDE FILE="menugeneral.html" -->
    --%>
    <%--     </div>--%>
    <!-- Secondary nav -->
    <%--    </div>
    --%>
    <!-- Sidebar ends -->
    <!-- Content begins -->
    <!-- Main content -->
    <div class="wrapper">
        <%--  <ul class="middleNavR" style="margin-top:0px;">
            <li><a href="#" onclick="mostrarcreacion();" class="tipN" original-title="Crear Tarjeta de ruta"><img src="../images/icons/middlenav/upload.png" alt=""></a></li>
            <li><a href="#" onclick="mostrarbusqueda();" class="tipN" original-title="Buscar Tarjeta de ruta"><img src="../images/icons/middlenav/add.png" alt=""></a></li>
    </ul>--%>
        <input type="hidden" runat="server" id="husuario" />
        <input type="hidden" runat="server" id="hagencia" />
        <!--DIV FLUID-->
        <div class="fluid">
            <!--DIV BUSQUEDA-->
            <div id="divbusqueda" class="widget grid12">
                <div class="whead">
                    <h6 id="titulo">
                        Busqueda Tarjeta de ruta</h6>
                </div>
                <!--PRIMERA FILA DE INGRESO-->
                <div class="formRow">
                    <!--NUMERO DE TARJETA DE RUTA-->
                    <div class="grid4">
                        <!--
                                             <input type="text" id="Especial"  name="Especial"  /> -->
                        <span class="note">No. Tarjeta de ruta</span>
                        <input id="txtnumerotarjeta" type="text" name="regular" />
                    </div>
                    <!--BOTON GENERAR-->
                    <div class="grid4" id="divbotonbusqueda">
                        <input type="button" value="Buscar" class="buttonS bLightBlue" id="btnbuscar" style="margin-top: 20px;" />
                        <input type="button" value="Reiniciar" class="buttonS bGold" id="btncancelar" style="margin-top: 20px;" />
                        <a class="navbar-brand" href="DashBoard.aspx">Regresar</a>
                        <img src="<% =Session("UrlImagen")%>" style="width: 90px" alt="" />
                        <%--                                    <a title="" class="leftUserDrop" style="margin-top: 50px;"><span id="usuario"></span>--%>
                        <span id="usuario"></span>
                    </div>
                    <div class="grid3">
                        <ul class="middleNavR" style="margin-top: -5px; width: 255px">
                            <li style="width: 60px; height: 60px;"><a href="#" onclick="mostrarbusqueda();" class="tipN"
                                original-title="Buscar Tarjeta de ruta" style="width: 73%; height: 74%; margin: -1px -2px -1px 0px;">
                                <img src="../images/icons/middlenav/bussy3.png" alt="BUSCAR TARJETA DE RUTA" style="margin: -15px 8px;
                                    height: 30px;" />
                            </a></li>
                            <!--
                                <li style="width: 60px; height: 60px;"><a href="#" onclick="mostrarcreacion();" class="tipN"
                                    original-title="Crear Tarjeta de ruta" style="width: 73%; height: 74%; margin: -1px -2px -1px 0px;">
                                    <img src="../images/icons/middlenav/add.png" alt="CREAR TARJETA DE RUTA" style="margin: -10px;" /></a></li>
                                    -->
                            <%--aqui quitar--%>
                            <%--
       <li style="width: 60px; height: 60px;"><a href="#" onclick="mostrarresumentarjetas();"
                                class="tipN" original-title="Resumen De Tarjetas" style="width: 73%; height: 74%;
                                margin: -1px -2px -1px 0px;">
                                <img src="../images/icons/middlenav/stats.png" alt="RESUMEN DE TARJETAS POR FECHA"
                                    style="margin: -10px;" /></a></li>
                            --%>
                        </ul>
                    </div>
                    <!--IMAGEN DE CARGA-->
                    <div id="cargar" class="ocultar" style="float: right; margin-top: -100px; margin-right: -200px;
                        font-weight: bold;">
                        Cargando...<img src="../images/elements/loaders/4s.gif" alt="" />
                    </div>
                </div>
                <!--FIN PRIMERA FILA-->
                <!--DIV FILA ENCABEZADO-->
                <div class="formRow" id="filaencabezado" style="display: none;">
                    <!--CORRELATIVO GENERADO POR EL SISTEMA-->
                    <div class="grid2">
                        <span class="note">No. Correlativo</span>
                        <input id="txtidtarjeta" type="text" disabled="disabled" name="regular" />
                    </div>
                    <!--FECHA DE CREACION-->
                    <div class="grid2">
                        <span class="note">Fecha creacion</span>
                        <input id="txtfechacreacione" type="text" disabled="disabled" name="regular" />
                    </div>
                    <!--USUARIO-->
                    <div class="grid2">
                        <span class="note">Usuario que creo</span>
                        <input id="txtusuarioe" type="text" disabled="disabled" name="regular" />
                    </div>
                    <!--AGENCIA-->
                    <div class="grid2">
                        <span class="note">Agencia</span>
                        <input id="txtagenciae" type="text" disabled="disabled" name="regular" />
                    </div>
                    <!--EMPRESA-->
                    <div class="grid2">
                        <span class="note">Empresa</span>
                        <input id="txtempresae" type="text" disabled="disabled" name="regular" />
                    </div>
                    <!--ESTADO-->
                    <div class="grid2">
                        <span class="note">Estado</span>
                        <input id="txttarjetaestado" type="text" disabled="disabled" name="regular" />
                    </div>
                </div>
                <!--FIN DIV FILA ENCABEZADO-->
                <!--INICIO DIV FILA DETALLE-->
                <div class="formRow" id="filaingresodetalle" style="display: none;">
                    <!--HORARIOS AGREGADOS-->
                    <div class="grid6">
                        <span class="note">HORARIOS</span>
                        <select id="drophorarioguardado" style="width: 100%;">
                        </select>
                    </div>
                    <!--CARGA-->
                    <div class="grid1" style="display: none">
                        <span class="note">Carga</span>
                        <input id="txtcarga" type="text" name="regular" />
                    </div>
                    <!--EXCURSION-->
                    <div class="grid1" style="display: none">
                        <span class="note">Excursion</span>
                        <input id="txtexcursion" type="text" name="regular" />
                    </div>
                    <!--VIATICOS PILOTO-->
                    <div class="grid2">
                        <span class="note">Viaticos Pil.</span>
                        <input id="txtviaticospiloto" style="text-align: right" type="text" name="regular" />
                    </div>
                    <!--VIATICOS ASISTENTE-->
                    <div class="grid2">
                        <span class="note">Viaticos Asis.</span>
                        <input id="txtviaticosasistente" style="text-align: right" type="text" name="regular" />
                    </div>
                    <!--NUMERO DE VALE-->
                    <div class="grid1" style="display: none">
                        <span class="note">Vale</span>
                        <input id="txtidvale" type="text" name="regular" />
                    </div>
                    <!--NUMERO DE VALE-->
                    <div class="grid1">
                        <input type="button" value="+" class="buttonS bRed" id="btnagregar" style="margin-top: 20px;" />
                    </div>
                </div>
                <!--INICIO DIV FILA DETALLE-->
                <div id="detalle" style="width: 100;">
                </div>
                <!--INICIO DIV PIE DEL DIV-->
                <div id="pie" style="width: 100; display: none;">
                    <div class="formRow">
                        <!--CERRAR TARJETA-->
                        <div class="grid">
                            <input type="button" value="Seleccionar Vehiculo" class="buttonS bRed" id="btnagregarhorario"
                                style="margin-top: 20px;" />
                            <input type="button" value="Facturas a Piloto (Origen)" class="buttonS bRed" id="btnagregartalonario"
                                style="margin-top: 20px;" />
                            <input type="button" value="Imprimir Entrega Facturas por Piloto" class="buttonS bRed"
                                id="btntarjetatalonario_entregafacturaspdf" style="margin-top: 20px;" />
                            <input type="button" value="Liquidar Facturas por Piloto" class="buttonS bGreen"
                                id="btnentregartalonario" style="margin-top: 20px;" />
                            <input type="button" value="Imprimir Liquidacion Facturas por Piloto" class="buttonS bGreen"
                                id="btntarjetatalonario_liquidarfacturaspdf" style="margin-top: 20px;" />
                            <input type="button" value="Horarios asociados" class="buttonS bLightBlue" id="btnhorarios"
                                style="margin-top: 20px; display: none;" />
                            <input type="button" value="Resumen Tarjeta" class="buttonS bLightBlue" id="btnresumen"
                                style="margin-top: 20px;" />
                            <input type="button" value="Crear Vale De Ruta Electronico" class="buttonS bGold"
                                id="btnvaleelectronico" style="margin-top: 20px;" />
                            <input type="button" value="Cerrar Tarjeta" class="buttonS bGold" id="btncerrartarjeta"
                                style="margin-top: 20px;" />
                        </div>
                        <!--TALONARIO-->
                        <div class="grid2">
                        </div>
                    </div>
                </div>
            </div>
            <!--FIN DEL DIV BUSQUEDA-->
        </div>
        <!--FIN DIV DEL FLUID-->
        <!--FIELDSET PARA EL MODAL DE FACTURACION  1000px  -->
        <fieldset id="boletos" style="display: none; width: 1000px;">
            <div class="fluid">
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Ingreso de Boletos vendidos en ruta</h6>
                    </div>
                    <!--CLIENTE-->
                    <div class="formRow">
                        <input type="hidden" id="hcorrelativo" value="0" />
                        <div class="grid4">
                            <span class="note">SERIE VENDIDA</span>
                            <select id="dropserieventa" style="margin-left: -10px;">
                            </select>
                        </div>
                        <div class="grid5">
                            <span class="note">ULTIMO CORRELATIVO VENDIDO</span>
                            <input type="text" id="txtultimocorrelativo" />
                        </div>
                        <div class="grid2">
                            <input type="button" value="Generar" id="btngenerarseries" class="buttonS bLightBlue"
                                style="margin-top: 20px;" />
                        </div>
                        <!--IMAGEN DE CARGA-->
                        <div id="cargar1" class="ocultar" style="float: right; margin-top: -30px;">
                            <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                        </div>
                    </div>
                    <div id="detalleboletos">
                    </div>
                    <div class="formRow">
                        <div class="grid12">
                            <input type="button" id="btnguardarboletos" value="Guardar" class="buttonS bGold" />
                            <div id="cargarboletos" class="ocultar">
                                Cargando...<img src="../images/elements/loaders/4s.gif" alt="" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Boletos guardados</h6>
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    BOLETO
                                </td>
                                <td>
                                    FACTURA
                                </td>
                                <td>
                                    SERIE
                                </td>
                                <td>
                                    ESTADO
                                </td>
                                <td>
                                    USUARIO
                                </td>
                                <!--<td>FECHA CREACION</td>-->
                                <td>
                                    TOTAL
                                </td>
                            </tr>
                        </thead>
                        <tbody id="boletosguardados">
                        </tbody>
                    </table>
                </div>
            </div>
        </fieldset>
        <!--FIN PARA EL MODAL DE FACTURACION-->
        <fieldset id="carga" style="display: none; width: 1000px;">
            <div class="fluid">
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Ingreso De Facturas De Carga En Ruta</h6>
                    </div>
                    <!--CLIENTE-->
                    <div class="formRow">
                        <input type="hidden" id="Hidden2" value="0" />
                        <div class="grid4">
                            <span class="note">SERIE VENDIDA</span>
                            <select id="dropseriecarga" style="margin-left: -10px;">
                            </select>
                        </div>
                        <div class="grid5">
                            <span class="note">ULTIMO CORRELATIVO VENDIDO</span>
                            <input type="text" id="txtultimocorrelativocarga" />
                        </div>
                        <div class="grid2">
                            <input type="button" value="Generar" id="btngenerarseriescarga" class="buttonS bLightBlue"
                                style="margin-top: 20px;" />
                        </div>
                        <!--IMAGEN DE CARGA-->
                        <div id="Div4" class="ocultar" style="float: right; margin-top: -30px;">
                            <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                        </div>
                    </div>
                    <div id="detallecarga">
                    </div>
                    <div class="formRow">
                        <div class="grid12">
                            <input type="button" id="btnguardarcarga" value="Guardar" class="buttonS bGold" />
                            <div id="Div6" class="ocultar">
                                Cargando...<img src="../images/elements/loaders/4s.gif" alt="" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Facturas De Carga En Ruta Guardadas</h6>
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    ID CARGA
                                </td>
                                <td>
                                    FACTURA
                                </td>
                                <td>
                                    SERIE
                                </td>
                                <td>
                                    ESTADO
                                </td>
                                <td>
                                    USUARIO
                                </td>
                                <!--<td>FECHA CREACION</td>-->
                                <td>
                                    TOTAL
                                </td>
                            </tr>
                        </thead>
                        <tbody id="cargaguardada">
                        </tbody>
                    </table>
                </div>
            </div>
        </fieldset>
        <fieldset id="cargaagencia" style="display: none; width: 1000px;">
            <div class="fluid">
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Ingreso De Facturas De Carga En Agencia</h6>
                    </div>
                    <!--CLIENTE-->
                    <div class="formRow">
                        <p>
                        </p>
                        <p>
                        </p>
                        <input type="hidden" id="Hidden4" value="0" />
                        <div class="grid5">
                            <span class="note" id="TituloTotalCargaAgencia">INGRESAR MONTO CARGA AGENCIA</span>
                        </div>
                        <div class="grid4">
                            <input type="text" id="TxtTotalCargaAgencia" />
                        </div>
                        <div class="grid2">
                            <input type="button" value="Guardar" id="BtnGuardarCargaAgencia" onclick="GuardarCargaAgencia(this.id);"
                                class="buttonS bLightBlue" style="" />
                        </div>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <div id="Div9" class="ocultar" style="float: right; margin-top: -30px;">
                            <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                        </div>
                    </div>
                    <div id="Div10">
                    </div>
                    <div class="formRow noBorderB">
                    </div>
                </div>
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            RESUMEN</h6>
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    TOTAL CARGA DE AGENCIA REGISTRADA
                                </td>
                                <td id="tdcargaagencia">
                                    200.00
                                </td>
                            </tr>
                        </thead>
                        <tbody id="Tbody1">
                        </tbody>
                    </table>
                </div>
            </div>
        </fieldset>
        <fieldset id="seleccioncarga" style="display: none; width: 850px;">
            <div class="fluid">
                <div class="widget grid6" style="margin-top: 0px; width: 850px;">
                    <div class="whead">
                        <h6>
                            Seleccionar Tipo De Carga</h6>
                    </div>
                    <!--CLIENTE-->
                    <div class="formRow">
                        <input type="hidden" id="Hidden3" value="0" />
                        <div class="grid4">
                            <span class="note">SELECCIONAR TIPO DE CARGA</span>
                        </div>
                        <div class="grid5">
                            <p id="numerocarga" style="display: none">
                            </p>
                        </div>
                        <div class="grid5">
                            <input type="button" value="Carga En Agencia" id="BtnSeleccionCargaAgencia" onclick="modalcargaagencia(this.id);"
                                class="buttonS bLightBlue" style="margin-top: 20px;" />
                            <input type="button" value="  Carga En Ruta    " onclick="modalcarga(this.id);" id="BtnSeleccionCargaRuta"
                                class="buttonS bGold" />
                        </div>
                        <!--IMAGEN DE CARGA-->
                        <div id="Div5" class="ocultar" style="float: right; margin-top: -30px;">
                            <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                        </div>
                    </div>
                    <div id="Div7">
                    </div>
                    <div class="formRow noBorderB">
                        <div class="grid12">
                            <div id="Div8" class="ocultar">
                                Cargando...<img src="../images/elements/loaders/4s.gif" alt="" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset id="boletosagencia" style="display: none; width: 1000px;">
            <div class="fluid">
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Ingreso de Boletos vendidos en agencia</h6>
                    </div>
                    <!--CLIENTE-->
                    <div class="formRow">
                        <input type="hidden" id="Hidden1" value="0" />
                        <div class="grid4" style="width: 15%">
                            <p>
                            </p>
                        </div>
                        <div class="grid5">
                            <span class="note" style="margin-bottom: 5px; margin-left: -10px;">NUMEROS DE BOLETOS
                                VENDIDOS EN AGENCIAS</span>
                            <input type="text" id="txtboletosagencia" style="margin-left: 30px;" />
                            <script type="text/javascript">
                                $('#txtboletosagencia').keypress(function (event) {
                                    var keycode = (event.keyCode ? event.keyCode : event.which);
                                    if (keycode == '13') {

                                        if (contaclick == 0) {
                                            $('#btnguardarboletosagencia').click();
                                            contaclick = 1;

                                        }
                                    }
                                    event.stopPropagation();
                                });
                            </script>
                        </div>
                        <div class="grid2">
                            <input type="button" value="Registrar" id="btnguardarboletosagencia" class="buttonS bLightBlue"
                                style="margin-top: 28px; margin-left: 30px;" />
                            <script type="text/javascript">
                                $('#btnguardarboletosagencia').keypress(function (event) {
                                    var keycode = (event.keyCode ? event.keyCode : event.which);
                                    if (keycode == '13') {
                                        document.getElementById('txtboletosagencia').focus();
                                        contaclick = 0;
                                    }
                                    event.stopPropagation();
                                });
                            </script>
                        </div>
                        <!--IMAGEN DE CARGA-->
                        <div id="Div1" class="ocultar" style="float: right; margin-top: -30px;">
                            <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                        </div>
                    </div>
                    <div id="Div3">
                    </div>
                    <div class="formRow">
                    </div>
                </div>
                <div class="widget grid6" style="margin-top: 0px;">
                    <div class="whead">
                        <h6>
                            Boletos guardados</h6>
                        <input type="button" value="Liberar Horario" id="btnliberarhorario" class="buttonS bRed"
                            style="margin-top: 8px; margin-left: 90px; margin-bottom: 5px" />
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    BOLETO
                                </td>
                                <td>
                                    FACTURA
                                </td>
                                <td>
                                    SERIE
                                </td>
                                <td>
                                    ESTADO
                                </td>
                                <td>
                                    USUARIO
                                </td>
                                <!--<td>FECHA CREACION</td>-->
                                <td>
                                    TOTAL
                                </td>
                            </tr>
                        </thead>
                        <tbody id="boletosguardadosagencia">
                        </tbody>
                    </table>
                </div>
            </div>
        </fieldset>
        <!--FIELDSET PARA EL MODAL DE HORARIOS RELACIONADOS-->
        <fieldset id="horarios" style="display: none;">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px;">
                <div class="whead">
                    <h6>
                        Horarios relacionados</h6>
                </div>
                <table id="tresumenhorariose" cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                    <thead>
                        <tr>
                            <td>
                                SELECCIONAR
                            </td>
                            <td>
                                FECHA
                            </td>
                            <td>
                                HORA
                            </td>
                            <td>
                                SERVICIO
                            </td>
                            <td>
                                RUTA
                            </td>
                            <td>
                                PILOTO
                            </td>
                            <td>
                                ASISTENTE
                            </td>
                        </tr>
                    </thead>
                    <tbody id="tresumenhorarios">
                    </tbody>
                </table>
                <table id="tmontos" cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                    <thead>
                        <tr>
                            <td>
                                ORIGEN
                            </td>
                            <td>
                                BOLETOS
                            </td>
                            <td>
                                TOTAL Q.
                            </td>
                        </tr>
                    </thead>
                    <tbody id="tmontosdetalle">
                    </tbody>
                </table>
                <div class="tableFooter">
                    <input type="button" value="Regresar" id="btnregresar" class="buttonS bLightBlue"
                        style="display: none;" />
                </div>
            </div>
        </fieldset>
        <!--FIELDSET PARA EL MODAL DE AGREGAR HORARIOS-->
        <fieldset id="agregarhorarios" style="display: none;">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px;">
                <div class="whead">
                    <h6 id="titulohorarios">
                        Agregar horarios de vehiculo a Tarjeta de ruta</h6>
                </div>
                <!--CLIENTE-->
                <div class="formRow" id="divparametroshorarios">
                    <div class="grid3">
                        <span class="note">SELECCIONAR VEHICULO</span>
                        <select id="DropVehiculo" style="width: 100%;">
                        </select>
                    </div>
                    <!--ORIGEN-->
                    <div class="grid3">
                    </div>
                    <!--HORARIOS-->
                    <div class="grid3">
                    </div>
                    <div class="grid2">
                    </div>
                    <!--FECHA VIAJE-->
                    <div class="grid3">
                        <span class="note">FECHA DE VIAJE</span>
                        <input id="txtfechaviaje" type="text" name="regular" />
                    </div>
                    <!--ORIGEN-->
                    <div class="grid3" style="display: none">
                        <span class="note">ORIGEN</span>
                        <select id="droporigen" style="width: 100%;">
                        </select>
                    </div>
                    <!--TIPOCOBRORUTA-->
                    <div class="grid3" style="display: none">
                        <span class="note">TIPOCOBRORUTA</span>
                        <select id="droptipocobroruta" style="width: 100%;">
                        </select>
                    </div>
                    <!--HORARIOS-->
                    <div class="grid3" style="display: none">
                        <span class="note">HORARIOS</span>
                        <select id="drophorario" style="width: 200px">
                        </select>
                    </div>
                    <div class="grid2">
                        <input type="button" value="Asignar Horario" id="btnasignarhorario" class="buttonS bLightBlue"
                            style="margin-top: 20px;" />
                    </div>
                    <!--IMAGEN DE CARGA-->
                    <div id="cargaragregarhorarios" class="ocultar" style="float: right; margin-top: -30px;">
                        <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                    </div>
                </div>
                <div id="Div2">
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    Horarios Vehiculo
                                </td>
                            </tr>
                        </thead>
                        <tbody id="dhorariosasignados">
                        </tbody>
                    </table>
                    <div class="tableFooter">
                    </div>
                </div>
            </div>
        </fieldset>
        <!--FIELDSET PARA EL MODAL DE AGREGAR TALONARIO   modal talonario asignarserie()         -->
        <fieldset id="talonario" style="display: none;">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px;">
                <div class="whead">
                    <h6>
                        AGREGAR TALONARIOS A RUTA</h6>
                </div>
                <!--CLIENTE-->
                <div class="formRow">
                    <div class="grid3">
                        <span class="note">SELECCIONE SERIE</span>
                        <select id="dropserie">
                        </select>
                    </div>
                    <%--                        AQUI LO NUEVO --%>
                    <div class="grid3">
                        <span class="note">CORRELATIVO INICIAL</span>
                        <input type="number" id="txtcorrelativoinicial" value="0" />
                    </div>
                    <div class="grid3">
                        <span class="note">CORRELATIVO FINAL</span>
                        <input type="number" id="txtcorrelativofinal" value="0" />
                    </div>
                    <%--                        END AQUI LO NUEVO --%>
                    <div class="grid2">
                        <input type="button" value="Asignar Serie" id="btnasignarserie" class="buttonS bLightBlue"
                            style="margin-top: 20px;" />
                    </div>
                    <!--IMAGEN DE CARGA-->
                    <div id="cargar2" class="ocultar" style="float: right; margin-top: -30px;">
                        <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                    </div>
                </div>
                <div id="detalletalonarios">
                    <div>
                        <a href="#" title="" class="logo">
                            <img src="../images/logo.png" style="width: 65px" alt=""></a>
                    </div>
                    <br>
                    <label for="male">
                        Constancia Serie Correlativos</label>
                    <br>
                    <div class="grid12">
                        <input id="txtturno" type="text" value="" disabled="disabled" size="200" />
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    SERIE
                                </td>
                                <td>
                                    USUARIO
                                </td>
                                <td>
                                    FECHA CREACION
                                </td>
                                <td>
                                    INICIAL
                                </td>
                                <td>
                                    FINAL
                                </td>
                            </tr>
                        </thead>
                        <tbody id="detalletalonariosd">
                        </tbody>
                    </table>
                    <div class="tableFooter">
                    </div>
                    <div>
                    </div>
                    <div>
                    </div>
                    <div>
                    </div>
                    <div>
                        <table>
                            <thead>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        <label for="male">
                                        </label>
                                        <br>
                                        <label for="male">
                                        </label>
                                        <br>
                                        <input id="txtpilotofirma" type="text" value="" name="regular" disabled="disabled"
                                            size="200" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </thead>
                            <tbody id="Tbody2">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--                         un boton -->
                <div class="grid2">
                    <%--                        <input type="button" value="Imprimir Constancia Serie Correlativos" id="btnImprimirConstanciaSerieCorrelativos"
                            class="buttonS bLightBlue" style="margin-top: 20px;" onclick="javascript:imprSelec('detalletalonarios')" />

                    --%>
                    <%--                        <a href="javascript:void(0)" id="imprime">Imprime</a>--%>
                </div>
            </div>
        </fieldset>
        <!--FIELDSET PARA EL MODAL DE ENTREGAR TALONARIO inicia  -->
        <fieldset id="talonario2" style="display: none;">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px;">
                <div class="whead">
                    <h6>
                        ENTREGA DE FACTURAS POR EL ASISTENTE
                    </h6>
                </div>
                <!--CLIENTE-->
                <div class="formRow">
                    <div class="grid3">
                        <span class="note">SELECCIONE SERIE</span>
                        <select id="dropserie2">
                        </select>
                    </div>
                    <%--                        AQUI LO NUEVO --%>
                    <div class="grid3">
                        <span class="note">CORRELATIVO INICIAL</span>
                        <input type="number" id="txtcorrelativoinicial2" value="0" />
                        <input type="hidden" id="hidcorrelativoinicial2" value="0" />
                    </div>
                    <div class="grid3">
                        <span class="note">CORRELATIVO FINAL</span>
                        <input type="number" id="txtcorrelativofinal2" value="0" />
                        <input type="hidden" id="hidcorrelativofinal2" value="0" />
                    </div>
                    <%--                        AQUI LO NUEVO --%>
                    <div class="grid3">
                        <span class="note">CORRELATIVO UTILIZADO</span>
                        <div id="utilizado">
                        </div>
                    </div>
                    <div class="grid3">
                        <span class="note">VALOR FACTURAS</span>
                        <input type="number" id="txtvalorfacturas" value="0" />
                    </div>
                    <%--                        <div class="grid3">
                            <span class="note">FACTURAS UTILIZADAS</span>
                            <input type="number" id="txtfacturasutilizadas" value="0" />
                        </div>--%>
                    <%--                        END AQUI LO NUEVO --%>
                    <div class="grid2">
                        <input type="button" value="Registra" id="btnasignarserie2" class="buttonS bLightBlue"
                            style="margin-top: 20px;" />
                    </div>
                    <!--IMAGEN DE CARGA-->
                    <div id="Div11" class="ocultar" style="float: right; margin-top: -30px;">
                        <img src="../images/elements/loaders/4s.gif" style="float: right;" alt="" />
                    </div>
                </div>
                <div id="detalletalonarios2">
                    <div>
                        <a href="#" title="" class="logo">
                            <img src="../images/logo.png" style="width: 65px" alt=""></a>
                    </div>
                    <br>
                    <label for="male">
                        Constancia Serie Correlativos</label>
                    <br>
                    <div class="grid12">
                        <input id="txtturno2" type="text" value="" disabled="disabled" size="200" />
                    </div>
                    <table cellpadding="0" cellspacing="0" width="100%" class="tLight noBorderT">
                        <thead>
                            <tr>
                                <td>
                                    SERIE
                                </td>
                                <td>
                                    USUARIO
                                </td>
                                <td>
                                    FECHA CREACION
                                </td>
                                <td>
                                    INICIAL
                                </td>
                                <td>
                                    FINAL
                                </td>
                            </tr>
                        </thead>
                        <tbody id="detalletalonariosd2">
                        </tbody>
                    </table>
                    <div class="tableFooter">
                    </div>
                    <div>
                    </div>
                    <div>
                    </div>
                    <div>
                    </div>
                    <div>
                        <table>
                            <thead>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        <label for="male">
                                        </label>
                                        <br>
                                        <label for="male">
                                        </label>
                                        <br>
                                        <input id="txtpilotofirma2" type="text" value="" name="regular" disabled="disabled"
                                            size="200" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </thead>
                            <tbody id="Tbody22">
                            </tbody>
                        </table>
                    </div>
                </div>
                <!--                         un boton -->
                <div class="grid2">
                    <%--                        <input type="button" value="Imprimir Constancia Serie Correlativos" id="btnImprimirConstanciaSerieCorrelativos2"
                            class="buttonS bLightBlue" style="margin-top: 20px;" onclick="javascript:imprSelec('detalletalonarios')" />

                    --%>
                    <%--                        <a href="javascript:void(0)" id="imprime2">Imprime</a>--%>
                </div>
            </div>
        </fieldset>
        <fieldset id="vales" style="display: none; width: 720px; height: 330px">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px; margin-left: 11px;">
                <div class="whead">
                    <h6 id="H1">
                        Vales De Ruta</h6>
                </div>
                <!--CLIENTE-->
                <div class="formRow" id="divvale">
                </div>
            </div>
        </fieldset>
        <fieldset id="valeselectronico" style="display: none; width: 720px; height: 330px">
            <div class="widget fluid grid6" style="margin-top: 0px; width: 700px; margin-left: 11px;">
                <div class="whead">
                    <h6 id="H2">
                        Vales De Ruta Electronico</h6>
                </div>
                <!--CLIENTE-->
                <div class="formRow" id="divvaleelectronico">
                </div>
            </div>
        </fieldset>
    </div>
    <!--FIN DEL DIV WRAPPER-->
    </div>
    <!-- Content ends -->
</body>
</html>
