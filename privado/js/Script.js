//FUNCION PARA MOSTRAR LA *******FECHA Y HORA ACTUAL****** DEL SERVIDOR
var GlobalOrigen;

function muestrareservas() {
    setTimeout("mostrardivreservado()",1000);
    setTimeout("document.getElementById('DropHoraReservado').focus();", 1500);
}

function muestraboletosdeida() {
    setTimeout("mostrardivprincipal();", 1000);
    setTimeout("document.getElementById('DropHora').focus();", 1500);
}

function muestrareasignacion() {
    setTimeout("mostrardivreasignacion();", 1000);
    setTimeout("document.getElementById('TxtBoleto').focus();", 1500);
}

function mostrardivreservado() {

    document.getElementById('DropHoraReservado').focus();
    document.getElementById('fieldsetreservado').style.display = '';
    document.getElementById('contenedorreservado').style.display = ''; 
    document.getElementById('fieldsetida').style.display = 'none';
    document.getElementById('contenedorbus').style.display = 'none';
    document.getElementById('fieldsetreasignacion').style.display = 'none';
    document.getElementById('contenedorbus_reasignacion').style.display = 'none';
    document.getElementById('fieldsetretorno').style.display = 'none';
    document.getElementById('contenedorbusvuelta').style.display = 'none';
}

 function mostrardivprincipal() {
     document.getElementById('DropHora').focus();
     document.getElementById('fieldsetida').style.display = '';
     document.getElementById('contenedorbus').style.display = '';
     document.getElementById('fieldsetreservado').style.display = 'none';
     document.getElementById('contenedorreservado').style.display = 'none';
     document.getElementById('fieldsetreasignacion').style.display = 'none';
     document.getElementById('contenedorbus_reasignacion').style.display = 'none';
     document.getElementById('fieldsetretorno').style.display = 'none';
     document.getElementById('contenedorbusvuelta').style.display = 'none';
            
  }

  function mostrardivreasignacion() {
      document.getElementById('DropHoraReasignar').focus();
      document.getElementById('fieldsetreasignacion').style.display = '';
      document.getElementById('contenedorbus_reasignacion').style.display = '';
      document.getElementById('fieldsetida').style.display = 'none';
      document.getElementById('contenedorbus').style.display = 'none';
      document.getElementById('fieldsetreservado').style.display = 'none';
      document.getElementById('contenedorreservado').style.display = 'none';
      document.getElementById('fieldsetretorno').style.display = 'none';
      document.getElementById('contenedorbusvuelta').style.display = 'none';
   }

function mostrarmodal(control) {
    $.fancybox.open("#" + control);
}
function cerrarmodal(control) {
    $.fancybox.close("#" + control)
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
function cargarTodosLosTurnos() {
    var origen = $('#DropOrigen').val();
    var fecha = document.getElementById('TxtFecha').value;
    if (origen != "" && fecha != "") {
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/TurnoSugerido",
            data: '{Origen: "' + origen + '",Fecha: "' + fecha + '"}',
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
            success: resultadohorariosTodos,
            async: false,
            error: erroreshorarios
        });
    }
}
function resultadohorariosTodos(msg) {
    $("#DropHora").html("");
    $("#DropHoraReservado").html("");
    $("#DropHoraReasignar").html("");
    $("#DropHoraVuelta").html("");
    var seleccionar = false;
    $.each(msg.d, function (index) {
        var estado = this.estado;
        if ((estado == "SI" && seleccionar == false) || index==0) {
            $('#DropHora').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            $('#DropHoraReservado').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            $('#DropHoraReasignar').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            $('#DropHoraVuelta').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
        }
        else {
            $("#DropHora").append($("<option></option>").attr("value", this.datos).text(this.hora));
            $("#DropHoraReservado").append($("<option></option>").attr("value", this.datos).text(this.hora));
            $("#DropHoraReasignar").append($("<option></option>").attr("value", this.datos).text(this.hora));
            $("#DropHoraVuelta").append($("<option></option>").attr("value", this.datos).text(this.hora));
        }
    });
    GlobalOrigen = document.getElementById("DropOrigen").value;
}
function cargarTurno(){  
    var origen = $('#DropOrigen').val();
    var fecha = document.getElementById('TxtFecha').value;
    if (origen == GlobalOrigen) {
        document.getElementById("DropHora").focus();
    } else {
        if (origen != "" && fecha != "") {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/TurnoSugerido",
                data: '{Origen: "' + origen + '",Fecha: "' + fecha + '"}',
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
                success: resultadohorarios,
                async: false,
                error: erroreshorarios
            });
        }
    }
    GlobalOrigen = document.getElementById("DropOrigen").value;
}
    function cargarTurnoReasignar() {
        var origen = $('#DropOrigenReasignar').val();
        var fecha = document.getElementById('TxtFechaServicio').value;
        if (origen == GlobalOrigen) {
            document.getElementById("DropHora").focus();
        } else {
            if (origen != "" && fecha != "") {
                $.ajax({
                    type: "POST",
                    url: "WSboleto.asmx/TurnoSugerido",
                    data: '{Origen: "' + origen + '",Fecha: "' + fecha + '"}',
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
                    success: resultadohorariosReasignar,
                    async: false,
                    error: erroreshorarios
                });
            }
        }
        GlobalOrigen = document.getElementById("DropOrigenReasignar").value;
    }
    function cargarTurnoReserva() {
        var origen = $('#DropOrigenReservado').val();
        var fecha = document.getElementById('TxtFechaReservado').value;
        if (origen == GlobalOrigen) {
            document.getElementById("DropHora").focus();
        } else {
            if (origen != "" && fecha != "") {
                $.ajax({
                    type: "POST",
                    url: "WSboleto.asmx/TurnoSugerido",
                    data: '{Origen: "' + origen + '",Fecha: "' + fecha + '"}',
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
                    success: resultadohorariosReservar,
                    async: false,
                    error: erroreshorarios
                });
            }
        }
        GlobalOrigen = document.getElementById("DropOrigenReservado").value;
    }
    function cargarTurnoVuelta() {
        var origen = $('#DropOrigenVuelta').val();
        var fecha = document.getElementById('TxtFechaVuelta').value;
        if (origen == GlobalOrigen) {
            document.getElementById("DropHoraVuelta").focus();
        } else {
            if (origen != "" && fecha != "") {
                $.ajax({
                    type: "POST",
                    url: "WSboleto.asmx/TurnoSugerido",
                    data: '{Origen: "' + origen + '",Fecha: "' + fecha + '"}',
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
                    success: resultadohorariosVuelta,
                    async: false,
                    error: erroreshorarios
                });
            }
        }
        GlobalOrigen = document.getElementById("DropOrigenVuelta").value;
    }
/********************************************************************************
    *********FUNCION QUE DESPLIEGA LOS HORARIOS RETORNADOS EN EL DROP HORA**********
    ********************************************************************************/
    function resultadohorarios(msg) {
        $("#DropHora").html("");
        var seleccionar=false;
        $.each(msg.d, function (index) {
            var estado = this.estado;
            if ((estado == "SI" && seleccionar == false) || index == 0) {
                $('#DropHora').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            }
            else
                $("#DropHora").append($("<option></option>").attr("value", this.datos).text(this.hora));
        });
    }
    function resultadohorariosVuelta(msg) {
        $("#DropHoraVuelta").html("");
        var seleccionar = false;
        $.each(msg.d, function (index) {
            var estado = this.estado;
            if ((estado == "SI" && seleccionar == false) || index == 0) {
                $('#DropHoraVuelta').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            }
            else
                $("#DropHoraVuelta").append($("<option></option>").attr("value", this.datos).text(this.hora));
        });
    }
    function resultadohorariosReasignar(msg) {
        $("#DropHoraReasignar").html("");
        var seleccionar = false;
        $.each(msg.d, function (index) {
            var estado = this.estado;
            if ((estado == "SI" && seleccionar == false) || index == 0) {
                $('#DropHoraReasignar').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            }
            else
                $("#DropHoraReasignar").append($("<option></option>").attr("value", this.datos).text(this.hora));
        });
    }
    function resultadohorariosReservar(msg) {
        $("#DropHoraReservado").html("");
        var seleccionar = false;
        $.each(msg.d, function (index) {
            var estado = this.estado;
            if ((estado == "SI" && seleccionar == false) || index == 0) {
                $('#DropHoraReservado').append('<option value="' + this.datos + '" selected="selected">' + this.hora + '</option>');
            }
            else
                $("#DropHoraReservado").append($("<option></option>").attr("value", this.datos).text(this.hora));
        });
    }
    function erroreshorarios(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }

    $(function () {
        $("#TxtFecha, #TxtFechaVuelta, #TxtFechaReservado, #txtfechaviajeo, #TxtFechaServicio").bind('blur', function () {
            GlobalOrigen = 0;
        });
        $("#TxtFecha").datepicker({
            numberOfMonths: 1,
            dateFormat: 'dd/mm/yy',
            firstDay: 0,
            onSelect: function (selectedDate) {
                document.getElementById('DropOrigen').focus();
            }
        });
        $("#TxtFechaVuelta").datepicker({
            numberOfMonths: 1,
            dateFormat: 'dd/mm/yy',
            firstDay: 0,
            onSelect: function (selectedDate) {
                document.getElementById('DropOrigenVuelta').focus();
            }
        });
        $("#TxtFechaReservado").datepicker({
            numberOfMonths: 1,
            dateFormat: 'dd/mm/yy',
            firstDay: 0,
            onSelect: function (selectedDate) {
                document.getElementById('DropOrigenReservado').focus();
            }
        });

        
        $("#TxtFechaServicio").datepicker({
            numberOfMonths: 1,
            dateFormat: 'dd/mm/yy',
            firstDay: 0,
            onSelect: function (selectedDate) {
                document.getElementById('DropOrigenReasignar').focus();
            }
        });
    });
function mensajecorrecto(mensaje) {
    $.jGrowl(mensaje, { theme: 'success', position: 'bottom-right' });
}

function mensajeadvertencia(mensaje) {
    $.jGrowl(mensaje, { theme: 'warning', position: 'bottom-right' });
}
function mensajeerror(mensaje) {
    $.jGrowl(mensaje, { theme: 'error', position: 'bottom-right'});
}

function mensajeerrorpermanente(mensaje) {
    $.jGrowl(mensaje, { theme: 'error', position: 'bottom-right', sticky: true });
}

var ventanapopup = 0;
var asientosdeida = "";
var destinodeagencia = "";
var seguro = 3;
var activarautoupdate;
var idhorarioocupado = 0;
var idrutaocupado = 0;
var idempresaocupado = 0;
var idservicioocupado = 0;
var idorigenocupado = 0;
var iddestinoocupado = 0;
var butacaocupadocrear = 0;
var butacaocupadoanular = 0;
var fechaviajeocupado = "";
var idboletoanular = "";
var origenocupado = "";
var origendestino = "";
var horarioocupado = "";
var recargocambiohorario = 0;
/*FIN VARIABLES GLOBALES PARA CAMBIO DE HORARIO DE UNA BUTACA*/
var id;
//FUNCION QUE SELECCIONA EL LUGAR AL CUAL PERTENECE EL USUARIO CONECTADO
function seleccionaoption(val) {
    for (var indice = 0; indice < document.getElementById('DropOrigen').length; indice++) {
        if (document.getElementById('DropOrigen').options[indice].text == val) {
            document.getElementById('DropOrigen').selectedIndex = indice;
            break;
        }
    }
}
function focoinicial() {
    document.getElementById('DropHora').focus();
}
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: "WSboleto.asmx/FechaHora",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: resultadovarios,
        error: erroresvarios
    });

    function resultadovarios(msg) {
        $.each(msg.d, function () {
            document.getElementById('TxtFecha').value = this.fecha;
            document.getElementById("TxtFechaServicio").value = this.fecha;
            document.getElementById('TxtFechaReservado').value = document.getElementById('TxtFecha').value;
            seleccionaoption(this.destino);
        });
    }
    function erroresvarios(msg) {
        mensajeerrorpermanente("Error " + msg.responseText);
    }

    $.ajax({
        type: "POST",
        url: "WSboleto.asmx/OrigenAjax",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: resultadoorigenajax,
        error: erroresorigenajax
    });

    function resultadoorigenajax(msg) {
        var contador = 0;
        $.each(msg.d, function () {
            if (contador == 0) {
                $('#DropOrigen').append('<option value="' + this.iddestino + '" selected="selected">' + this.nombredestino + '</option>');
                $('#DropOrigenVuelta').append('<option value="' + this.iddestino + '" selected="selected">' + this.nombredestino + '</option>');
                $('#DropOrigenReservado').append('<option value="' + this.iddestino + '" selected="selected">' + this.nombredestino + '</option>');
                $('#DropOrigenReasignar').append('<option value="' + this.iddestino + '" selected="selected">' + this.nombredestino + '</option>');
            }
            else {
                $('#DropOrigen').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
                $('#DropOrigenVuelta').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
                $('#DropOrigenReservado').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
                $('#DropOrigenReasignar').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
            }
            contador++;
        });
        function verificaFechaOrigen() {
            var fecha = document.getElementById("TxtFecha").value;
            var origen = document.getElementById("DropOrigen").value;
            if (fecha != "" && origen != "") {
                cargarTodosLosTurnos();
            }
            else {
                setTimeout(verificaFechaOrigen(), 1000);
            }
        }
        verificaFechaOrigen();
    }
    function erroresorigenajax(msg) {
        mensajeerrorpermanente("Error " + msg.responseText);
    }
    function resultadodestinoajax(msg) {
        $.each(msg.d, function () {
            $('#DropDestino').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
            $('#DropDestinoVuelta').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
            $('#DropDestinoReservado').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
            $('#DropDestinoReasignar').append('<option value="' + this.iddestino + '">' + this.nombredestino + '</option>');
        });
    }
    function erroresdestinoajax(msg) {
        mensajeerrorpermanente("Error " + msg.responseText);
    }

    $("#btnliberarbutaca").click(function () {
        var butaca = document.getElementById("txtbutaca").value;
        var fecha = document.getElementById("TxtFecha").value;
        var origen = document.getElementById("DropOrigen").value;
        var destino = document.getElementById("DropDestino").value;
        var hora = document.getElementById("DropHora").value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/liberarButaca",
            data: '{butaca: "' + butaca + '", fecha:"' + fecha + '", origen:"' + origen + '", destino:"' + destino + '", hora:"' + hora + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var butaca = document.getElementById("txtbutaca").value;
                document.getElementById(butaca).className = "libre";
                document.getElementById(butaca).title = "";
                cerrarmodal('facturacion-modal');
            },
            error: function (data) {
            }
        });
    });

    $("input[type=text]").focus(function () {
        this.select();
    });
    $("#TxtDescuentoFac").blur(function () {
        var descuento = document.getElementById("TxtDescuentoFac").value;
        var subtotal = document.getElementById("totalida").innerHTML;
        var control = document.getElementById('lblcontrol').innerHTML;
        if (control == "ida") {
            var reasignar = document.getElementById("BtnImprimirReasginar").style.display;
            if (reasignar == "none") {
                if (isNaN(descuento) == false && descuento != "" && descuento > 0 && descuento <= parseFloat(subtotal)) {
                    subtotal -= descuento;
                    document.getElementById("TxtSubTotalFac").value = subtotal;
                } else if (descuento == "" || descuento == 0) {
                    document.getElementById("TxtSubTotalFac").value = document.getElementById("totalida").innerHTML;
                }
                else {
                    mensajeadvertencia("Descuento Invalido");
                }
            } else {
                var descuento = document.getElementById("TxtDescuentoFac").value;
                var subtotal = document.getElementById("TxtTotalFac").value;
                if (isNaN(descuento) == false && descuento != "" && descuento > 0 && descuento <= parseFloat(subtotal)) {
                    subtotal -= descuento;
                    document.getElementById("TxtSubTotalFac").value = subtotal;
                } else if (descuento == "" || descuento == 0) {
                    document.getElementById("TxtSubTotalFac").value = document.getElementById("totalida").innerHTML;
                }
                else {
                    mensajeadvertencia("Descuento Invalido");
                }
            }
        } else if (control == "vuelta") {
            subtotal = redondea(parseFloat(document.getElementById('totalida').innerHTML) + parseFloat(document.getElementById('totalvuelta').innerHTML));
            if (isNaN(descuento) == false && descuento != "" && descuento > 0 && descuento <= parseFloat(subtotal)) {
                subtotal -= descuento;
                document.getElementById("TxtSubTotalFac").value = subtotal;
            } else if (descuento == "" || descuento == 0) {
                document.getElementById("TxtSubTotalFac").value = subtotal;
            }
            else {
                mensajeadvertencia("Descuento Invalido");
            }
        } else if (control == "cambiarhorario") {
            var descuento = document.getElementById("TxtDescuentoFac").value;
            var subtotal = document.getElementById("TxtTotalFac").value;
            if (isNaN(descuento) == false && descuento != "" && descuento > 0 && descuento <= parseFloat(subtotal)) {
                subtotal -= descuento;
                document.getElementById("TxtSubTotalFac").value = subtotal;
            } else if (descuento == "" || descuento == 0) {
                document.getElementById("TxtSubTotalFac").value = document.getElementById("totalida").innerHTML;
            }
            else {
                mensajeadvertencia("Descuento Invalido");
            }
        }
    });



});
//DISMINUIR EL TOTAL DE RESERVACION AL QUITAR LA SELECCION DE UN CHECKBOX
function valorasiento(valor) {
    var botonasiento = document.getElementById(valor).title;
    var posicion = botonasiento.lastIndexOf("|");
    var total = botonasiento.substring(posicion + 8, botonasiento.length);
    return total;
}
function redondea(number) {
    var newnumber = Math.round(number * Math.pow(10, 2)) / Math.pow(10, 2);
    return newnumber.toFixed(2);
}
function disminuyetotalreservado(valor) {
    arreglo = valor.split("-");
    var total = valorasiento(arreglo[1]);
    var nuevototal = parseFloat(document.getElementById('totalreservacion').innerHTML);
    var nuevocheck = document.getElementById(valor);
    if (nuevocheck.checked == true) {
        nuevototal += parseFloat(total);
    }
    else {
        nuevototal -= parseFloat(total);
    }
    document.getElementById('totalreservacion').innerHTML = redondea(nuevototal);
}
/*LA MISMA FUNCION DE SELECCIONAR ASIENTOS PERO DE REGRESO*/
//SELECCIONA ASIENTO POR ASIENTO CON EL MOUSE
function seleccionaasientovuelta(control) {
    var nuevocontrol = document.getElementById(control);
    if ($("#" + nuevocontrol.id).is(".compra")) {
        document.getElementById(control).className = "libre";
        document.getElementById('TxtAsientosVuelta').value = "";
        $("#contenedorbusvuelta td .compra").each(function (index) {
            butaca = this.value;
            if (index >= 1) {
                document.getElementById('TxtAsientosVuelta').value = document.getElementById('TxtAsientosVuelta').value + "," + butaca;
            } else {
                document.getElementById('TxtAsientosVuelta').value = butaca;
            }
        });
    }
    else if ($("#" + nuevocontrol.id).is(".libre")) {
        document.getElementById(control).className = "compra";
        document.getElementById('TxtAsientosVuelta').value = "";
        $("#contenedorbusvuelta td .compra").each(function (index) {
            butaca = this.value;
            if (index >= 1) {
                document.getElementById('TxtAsientosVuelta').value = document.getElementById('TxtAsientosVuelta').value + "," + butaca;
            } else {
                document.getElementById('TxtAsientosVuelta').value = butaca;
            }
        });
    }  
}
//SELECCIONA ASIENTO POR ASIENTO CON EL MOUSE
function seleccionaasiento(control) {
    var nuevocontrol = document.getElementById(control);
    //*****************************ACCIONES A REALIZAR SI EL ASIENTO ESTA RESERVADO*************************************//
    if ($("#" + nuevocontrol.id).is(".reservado")) {
        var posicion = nuevocontrol.title.indexOf("|");
        var cliente = nuevocontrol.title.substring(0, posicion);
        var posicionfinal = nuevocontrol.title.lastIndexOf("|");
        var miTotal = nuevocontrol.title.split("|");
        var total = miTotal[2].split(" ");
        numerocaracteres = parseInt(nuevocontrol.title.length) - parseInt(posicionfinal);
        var viaje = nuevocontrol.title.substring(posicion + 10, parseInt(nuevocontrol.title.length) - numerocaracteres);
        var asientosreservados = "";
        var totalreservacion = 0;
        document.getElementById("chkasiento").innerHTML = "";
        $("td .reservado").each(function () {
            clienteAct = this.title.split("|", 1);
            if (clienteAct == cliente && isNaN(this.id)==false) {
                asientosreservados = asientosreservados + "," + this.id;
                var contenedor = document.getElementById('chkasiento');
                var chknew = document.createElement('div');
                chknew.setAttribute('id', 'checkboxs' + this.id);
                chknew.innerHTML = "<input type='checkbox' class='chks' name='chks' id='nuevo-" + this.id + "' checked='checked' onclick='disminuyetotalreservado(this.id);' />Asiento: " + this.id + " con valor de: " + redondea(total[1]) + " ";
                totalreservacion += parseFloat(total[1]);
                contenedor.appendChild(chknew);
            }
        });
        if (asientosreservados.charAt(0) == ',') {
            var largo = parseInt(asientosreservados.length)
            asientosreservados = asientosreservados.substring(1, largo);
        }
        var combo = document.getElementById("DropHora");
        var hora = combo.options[combo.selectedIndex].text.split(" | ");
        document.getElementById('TxtHoraR').value = hora[0];
        document.getElementById('totalreservacion').innerHTML = redondea(totalreservacion);
        document.getElementById('TxtAsientosR').value = asientosreservados;
        document.getElementById('TxtNombreR').value = cliente;
        document.getElementById('TxtFechaR').value = document.getElementById('TxtFecha').value;
        document.getElementById('TxtViajeR').value = viaje;
        mostrarmodal('basic-modal-content');

    }
    //************************ACCIONES A REALIZAR SI SE SELECCIONA UN ASIENTO OCUPADO*****************************//
    else if ($("#" + nuevocontrol.id).is(".ocupado")) {
    }
    else {
        var nuevoasiento = document.getElementById('TxtAsientos').value;
        if (asientoexiste(nuevocontrol.value) == true) {
            document.getElementById('TxtAsientos').value = removerasiento(nuevocontrol.value);
            nuevocontrol.className = "libre";
            seleccionaasientosmasivos(document.getElementById("TxtAsientos").value);
        }
        else {
            if (!nuevoasiento) {
                nuevoasiento = nuevocontrol.value;
            }
            else {
                nuevoasiento = nuevoasiento + ',' + nuevocontrol.value;
            }
            nuevocontrol.className = "compra";
            document.getElementById('TxtAsientos').value = nuevoasiento;
            seleccionaasientosmasivos(document.getElementById("TxtAsientos").value);
        }
    }
}
function seleccionaasientoreasignar(control) {
    var nuevocontrol = document.getElementById(control);
    //*****************************ACCIONES A REALIZAR SI EL ASIENTO ESTA RESERVADO*************************************//
    if ($("#" + nuevocontrol.id).is(".reservado")) {
    }
    //************************ACCIONES A REALIZAR SI SE SELECCIONA UN ASIENTO OCUPADO*****************************//
    else if ($("#" + nuevocontrol.id).is(".ocupado")) {
        boletocomprado(nuevocontrol.id);
    }
    else if ($("#" + nuevocontrol.id).is(".compra")) {
        nuevocontrol.className = "libre";
        document.getElementById('TxtAsientoAsignacion').value = "";
        $("td .compra").each(function (index) {
            butaca = this.value;
            if (index >= 1) {
                document.getElementById('TxtAsientoAsignacion').value = document.getElementById('TxtAsientoAsignacion').value + "," + butaca;
            } else {
                document.getElementById('TxtAsientoAsignacion').value = butaca;
            }
        });
    }
    else {
        var nuevoasiento = document.getElementById('TxtAsientoAsignacion').value;
        if (asientoexiste(nuevocontrol.value) == true) {
            document.getElementById('TxtAsientoAsignacion').value = removerasiento(nuevocontrol.value);
            nuevocontrol.className = "libre";
            seleccionaasientosmasivos(document.getElementById("TxtAsientoAsignacion").value);
        }
        else {
            if (!nuevoasiento) {
                nuevoasiento = nuevocontrol.value;
            }
            else {
                nuevoasiento = nuevoasiento + ',' + nuevocontrol.value;
            }
            document.getElementById('TxtAsientoAsignacion').value = "";
            nuevocontrol.className = "compra";
            $("td .compra").each(function (index) {
                butaca = this.value;
                if (index >= 1) {
                    document.getElementById('TxtAsientoAsignacion').value = document.getElementById('TxtAsientoAsignacion').value + "," + butaca;
                } else {
                    document.getElementById('TxtAsientoAsignacion').value = butaca;
                }
            });
        }
    }
}
//****************************************** B O L E T O S    D E  I D A **********************************//
//FUNCION QUE CAMBIA EL ESTADO DE LOS ASIENTOS AL ESCRIBIRLOS EN EL txtasientos BOLETOS DE IDA
function seleccionaasientosmasivos(valor) {
    var contador = 0;
    var totalbutacas = parseFloat(document.getElementById('thocupado').innerHTML)+parseFloat(document.getElementById('threservado').innerHTML)+parseFloat(document.getElementById('thlibre').innerHTML);
    if (!valor){
        valor = 0;
        $("#contenedorbus .compra").each(function (index) {
            var id = $(this).attr("id");
            document.getElementById(id).className = "libre";
        });
     }
    else {
        if (determinaasientoocupado(valor) == false) {
            contador = 0;
            if (!valor)
            { }
            else {
                var arreglo = valor.split(",");
                for (i = 0; i < arreglo.length; i++) {
                    var numeroasiento = arreglo[i];
                    if (numeroasiento <= totalbutacas) {
                        document.getElementById(numeroasiento).className = "compra";
                    } else if (numeroasiento > totalbutacas && numeroasiento <= totalbutacas + 10) {
                        contador++;
                    } else {
                        cerrarmodal('facturacion-modal');
                        mensajeadvertencia("El Exendente Es Mayor A La Capacidad Permitida");
                        document.getElementById("TxtAsientos").focus();
                        return;
                    }
                }
                //DETERMINAMOS SI LOS ASIENTOS INGRESADOS POSEEN LA CLASE COMPRA
                $("#contenedorbus .compra").each(function (index) {
                    var id = $(this).attr("id");
                    var existe = false;
                    for (i = 0; i < arreglo.length; i++) {
                        if (id == arreglo[i]) {
                            contador++;
                            existe = true;
                            break;
                        }
                    }
                    //SI EXISTE ES FALSO QUIERE DECIR QUE EL ASIENTO ESTA MARCADO COMO COMPRA Y NO ESTA INGRESADO
                    if (existe == false) {
                        $(this).removeClass("compra");
                        $(this).addClass("libre");
                    }
                });
            }
        }
        else {
            mensajeadvertencia("Uno o mas asientos ingresados esta ocupado");
            document.getElementById('TxtAsientos').focus();
        }
    }
    //MOSTRAMOS EL TOTAL POR LOS BOLETOS INGRESADOS O SELECCIONADOS........
    document.getElementById('totalida').innerHTML = redondea(parseFloat(document.getElementById('totalunitarioida').innerHTML) * parseFloat(contador));
}
function seleccionaasientosmasivosreasginar(valor) {
    var contador = 0;
    if (determinaasientoocupadoreasignar(valor) == false) {
        if (!valor)
        { }
        else {
            var arreglo = valor.split(",");
            if (arreglo.length > 1) {
                mensajeadvertencia("Solamente se puede seleccionar una butaca");
                document.getElementById("TxtAsientoAsignacion").focus();
                return;
            }
            for (i = 0; i < arreglo.length; i++) {
                var numeroasiento = arreglo[i];
                numeroasiento = "reasignar" + numeroasiento;
                document.getElementById(numeroasiento).className = "compra";
                contador++;
            }
            //DETERMINAMOS SI LOS ASIENTOS INGRESADOS POSEEN LA CLASE COMPRA
            $("#contenedorbus_reasignacion .compra").each(function (index) {
                var id = $(this).attr("id");
                var largo = id.length;
                id = id.substring(9, largo);
                var existe = false;
                for (i = 0; i < arreglo.length; i++) {
                    if (id == arreglo[i]) {
                        existe = true;
                       // break;
                    }
                }
                //SI EXISTE ES FALSO QUIERE DECIR QUE EL ASIENTO ESTA MARCADO COMO COMPRA Y NO ESTA INGRESADO
                //EN EL TEXTO TxtAsientos
                if (existe == false) {
                    $(this).removeClass("compra");
                    $(this).addClass("libre");
                }
            });
            //FIN DEL EACH
        }
    }
    else {
        mensajeadvertencia("Uno o más asientos ingresados esta ocupado");
        document.getElementById('TxtAsientoAsignacion').focus();
    }
    document.getElementById('totalReasignar').innerHTML = redondea(parseFloat(document.getElementById('precioReasignar').innerHTML) * parseFloat(contador));
}
//FUNCION QUE DETERMINA SI UN ASIENTO ESTA OCUPADO BOLETO_IDA
function determinaasientoocupado(valor) {
    var arreglo = valor.split(",");
    var existe = false;
    var existereservado = false;
    $("#contenedorbus .ocupado,#contenedorbus .reservado").each(function (index) {
        var id = $(this).attr("id");
        for (i = 0; i < arreglo.length; i++) {
            if (id == arreglo[i]) {
                existe = true;
                break;
            }
            if (existe == true) return false;
        }
    });
    if (existe == true)
        return true;
    else
        return false;
}
//FUNCION QUE DETERMINA SI UN ASIENTO YA HA SIDO ESCRITO O SELECCIONADO BOLETO_IDA
function asientoexiste(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientos').value;
    var asiento = asientos.split(",");
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] == nuevoasiento)
            existe = true;
        else
            existe = false;
        i++;
    }
    return existe;
}
//*************REMOVER***************** UN ASIENTO SI YA EXISTE **************BOLETO_IDA**************
function removerasiento(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientos').value;
    var asiento = asientos.split(",");
    var nuevosasientos = '';
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] != nuevoasiento) {
            if (i == 0) {
                nuevosasientos = asiento[i];
            }
            else {
                nuevosasientos = nuevosasientos + ',' + asiento[i];
            }
            existe = false;
        }
        i++;
    }
    //POR SI QUEDA UNA COMITA AL INICIO
    if (nuevosasientos.charAt(0) == ',') {
        var largo = parseInt(nuevosasientos.length)
        nuevosasientos = nuevosasientos.substring(1, largo);
    }
    return nuevosasientos;
}
//CAMBIA EL ESTATUS A LOS ASIENTOS QUE HAN SIDO VENDIDOS BOLETO_IDA
function cambioestatusasiento(valor) {
    if (!valor)
    { }
    else {
        var butacas = document.getElementById("butacasR").value;
        if (butacas != "") {
            var arreglo1 = butacas.split(",");
            var title = document.getElementById(arreglo1[0]).title;
            for (i = 0; i < arreglo1.length; i++) {
                var numeroasiento = arreglo1[i];
                document.getElementById(numeroasiento).className = "libre";
                document.getElementById(numeroasiento).title = "";
            }
            /*para que siga funcionando con boleto de ida y no reservaciones*/
            document.getElementById("butacasR").value = "";
        }
        var arreglo = valor.split(",");
        for (i = 0; i < arreglo.length; i++) {
            var numeroasiento = arreglo[i];
            document.getElementById(numeroasiento).className = "ocupado";
            document.getElementById(numeroasiento).title = title;
        }
    }
}
//********************************************* B O L E T O S   D E   V U E L T A **********************************//
function determinaasientoocupadovuelta(valor) {
    var arreglo = valor.split(",");
    var i = 0;
    var existe = false;
    var existereservado = false;
    $("#contenedorbusvuelta .ocupado,#contenedorbusvuelta .reservado").each(function (index) {
        var id = $(this).attr("id");
        for (i = 0; i < arreglo.length; i++) {
            var largo = id.length;
            id = id.substring(6, largo);
            if (id == arreglo[i]) {
                existe = true;
                break;
            }
            if (existe == true) return false;
        }
    });
    if (existe == true)
        return true;
    else
        return false;
}
function determinaasientoocupadoreasignar(valor) {
    var arreglo = valor.split(",");
    var i = 0;
    var existe = false;
    var existereservado = false;
    $("#contenedorbus_reasignacion .ocupado, #contenedorbus_reasignacion .reservado").each(function (index) {
        var id = $(this).attr("id");
        for (i = 0; i < arreglo.length; i++) {
            var largo = id.length;
            id = id.substring(9, largo);
            if (id == arreglo[i]) {
                existe = true;
                break;
            }
            if (existe == true) return false;
        }
    });
    if (existe == true)
        return true;
    else
        return false;
}
//********************SELECCIONA ASIENTOS ESCRITOS en TXTASIENTOS BOLETO_REGRESO***********************************
function seleccionaasientosmasivosvuelta(valor) {
    var contador = 0;
    var totalbutacas = parseFloat(document.getElementById('thocupado').innerHTML) + parseFloat(document.getElementById('threservado').innerHTML) + parseFloat(document.getElementById('thlibre').innerHTML);
    if (!valor) {
        valor = 0;
        $("#contenedorbusvuelta .compra").each(function (index) {
            var id = $(this).attr("id");
            document.getElementById(id).className = "libre";
        });
    }
    else {
        if (determinaasientoocupadovuelta(valor) == false) {
            contador = 0;
            if (!valor)
            { }
            else {
                $("#contenedorbusvuelta .compra").each(function (index) {
                    var id = $(this).attr("id");
                    document.getElementById(id).className = "libre";
                });
                var arreglo = valor.split(",");
                for (i = 0; i < arreglo.length; i++) {
                    var numeroasiento = arreglo[i];
                    if (numeroasiento <= totalbutacas) {
                        numeroasiento = 'Button' + numeroasiento;
                        document.getElementById(numeroasiento).className = 'compra';
                        contador++;
                    }else {
                        cerrarmodal('facturacion-modal');
                        mensajeadvertencia("No Se Puede Agregar Un Exendente A Los Boletos De Vuelta");
                        document.getElementById("TxtAsientosVuelta").focus();
                        return;
                    }
                }
                //DETERMINAMOS SI LOS ASIENTOS INGRESADOS POSEEN LA CLASE COMPRA
                $("#contenedorbusvuelta .compra").each(function (index) {
                    var id = $(this).attr("id");
                    var existe = false;
                    for (i = 0; i < arreglo.length; i++) {
                        if (id == arreglo[i]) {
                            contador++;
                            existe = true;
                            break;
                        }
                    }
                    //SI EXISTE ES FALSO QUIERE DECIR QUE EL ASIENTO ESTA MARCADO COMO COMPRA Y NO ESTA INGRESADO
                    if (existe == false) {
                        $(this).removeClass("libre");
                        $(this).addClass("compra");
                    }
                });
            }
        }
        else {
            mensajeadvertencia("Uno o mas asientos ingresados esta ocupado");
            document.getElementById('TxtAsientosVuelta').focus();
        }
    }
    //MOSTRAMOS EL TOTAL POR LOS BOLETOS INGRESADOS O SELECCIONADOS........
    document.getElementById('totalvuelta').innerHTML = redondea(parseFloat(document.getElementById('totalunitariovuelta').innerHTML) * parseFloat(contador));
}
function seleccionaasientosmasivosvueltaNO(valor) {
    var contador = 0;
    if (determinaasientoocupadovuelta(valor) == false) {
        if (!valor)
        { }
        else {
            var arreglo = valor.split(",");
            for (i = 0; i < arreglo.length; i++) {
                var numeroasiento = arreglo[i];
                numeroasiento = 'Button' + numeroasiento;
                document.getElementById(numeroasiento).className = "compra";
                contador++;
            }
            //DETERMINAMOS SI LOS ASIENTOS INGRESADOS POSEEN LA CLASE COMPRA
            $("#contenedorbusvuelta .compra").each(function (index) {
                var id = $(this).attr("id");
                var largo = id.length;
                id = id.substring(6, largo);
                var existe = false;
                for (i = 0; i < arreglo.length; i++) {
                    if (id == arreglo[i]) {
                        existe = true;
                        break;
                    }
                }
                //SI EXISTE ES FALSO QUIERE DECIR QUE EL ASIENTO ESTA MARCADO COMO COMPRA Y NO ESTA INGRESADO
                //EN EL TEXTO TxtAsientos
                if (existe == false) {
                    $(this).removeClass("compra");
                    $(this).addClass("libre");
                }
            });
            //FIN DEL EACH
        }
    }
    else {
        mensajeadvertencia("Uno o más asientos ingresados esta ocupado");
        document.getElementById('TxtAsientosVuelta').focus();
    }
    document.getElementById('totalvuelta').innerHTML = redondea(parseFloat(document.getElementById('totalunitariovuelta').innerHTML) * parseFloat(contador));
}
//DE VUELTA        //FUNCION QUE DETERMINA SI UN ASIENTO YA ESTA OCUPADO BOLETO_REGRESO
function seleccionaasientosmasivosreservado(valor) {
    var contador = 0;
    if (determinaasientoocupadoreservado(valor) == false) {
        if (!valor)
        { }
        else {
            var arreglo = valor.split(",");
            for (i = 0; i < arreglo.length; i++) {
                var numeroasiento = arreglo[i];
                numeroasiento = 'r' + numeroasiento;
                document.getElementById(numeroasiento).className = "compra";
                contador++;
            }
            //DETERMINAMOS SI LOS ASIENTOS INGRESADOS POSEEN LA CLASE COMPRA
            $("#contenedorreservado .compra").each(function (index) {
                var id = $(this).attr("id");
                var largo = id.length;
                id = id.substring(1, largo);
                var existe = false;
                for (i = 0; i < arreglo.length; i++) {
                    if (id == arreglo[i]) {
                        existe = true;
                        break;
                    }
                }
                //SI EXISTE ES FALSO QUIERE DECIR QUE EL ASIENTO ESTA MARCADO COMO COMPRA Y NO ESTA INGRESADO
                //EN EL TEXTO TxtAsientos
                if (existe == false) {
                    $(this).removeClass("compra");
                    $(this).addClass("libre");
                }
            });
            //FIN DEL EACH
        }
    }
    else {
        mensajeadvertencia("Uno o más asientos ingresados esta ocupado");
        document.getElementById('TxtAsientosReservado').focus();
    }
    document.getElementById('totalreservado').innerHTML = redondea(parseFloat(document.getElementById('totalunitarioreservado').innerHTML) * parseFloat(contador));
}
//FUNCION PARA REMOVER UN ASIENTO SI YA HA SIDO SELECCIONADO O ESCRITO BOLETO_REGRESO
function removerasientovuelta(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientosVuelta').value;
    var asiento = asientos.split(","); //27
    var nuevosasientos = '';
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] != nuevoasiento) {
            if (i == 0) {
                nuevosasientos = asiento[i];
            }
            else {
                nuevosasientos = nuevosasientos + ',' + asiento[i];
            }
            existe = false;
        }
        i++;
    }
    if (nuevosasientos.charAt(0) == ',') {
        var largo = parseInt(nuevosasientos.length)
        nuevosasientos = nuevosasientos.substring(1, largo);
    }
    return nuevosasientos;
}
function asientoexistevuelta(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientosVuelta').value;
    var asiento = asientos.split(",");
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] == nuevoasiento)
            existe = true;
        else
            existe = false;
        i++;
    }
    return existe;
}
//******************CAMBIA EL ESTATUS A LOS ASIENTOS VENDIDOS PERO LOS DE REGRESO BOLETO_REGRESO***************************
function cambioestatusasientovuelta(valor) {
    if (!valor)
    { }
    else {
        var arreglo = valor.split(",");
        for (i = 0; i < arreglo.length; i++) {
            var numeroasiento = arreglo[i];
            numeroasiento = 'Button' + numeroasiento;
            document.getElementById(numeroasiento).className = "ocupado";
            //document.getElementById(numeroasiento).disabled = true;
        }
    }
}
//FUNCION QUE DETERMINA SI UN ASIENTO YA ESTA OCUPADO BOLETO_REGRESO
function determinaasientoocupadoreservado(valor) {
    var arreglo = valor.split(",");
    var existe = false;
    $("#contenedorreservado .reservado,#contenedorreservado .ocupado").each(function (index) {
        var id = $(this).attr("id");
        var largo = id.length;
        id = id.substring(1, largo);
        for (i = 0; i < arreglo.length; i++) {
            if (id == arreglo[i]) {
                existe = true;
                break;
            }
            if (existe == true) return false;
        }
    });
    if (existe == true)
        return true;
    else
        return false;
}
//FUNCION QUE SELECCIONA UN ASIENTO CON EL CURSOR BOLETO_REGRESO
function seleccionaasientoreservado(control) {
    if ($("#" + control).is(".reservado")) {
        return;
    }
    var nuevocontrol = document.getElementById(control);
    var nuevoasiento = document.getElementById('TxtAsientosReservado').value;
    if (asientoexistereservado(nuevocontrol.value) == true) {
        document.getElementById('TxtAsientosReservado').value = removerasientoreservado(nuevocontrol.value);
        nuevocontrol.className = "libre";
        seleccionaasientosmasivosreservado(document.getElementById("TxtAsientosReservado").value);
    }
    else {
        if (!nuevoasiento) {
            nuevoasiento = nuevocontrol.value;
        }
        else {
            nuevoasiento = nuevoasiento + ',' + nuevocontrol.value;
        }
        nuevocontrol.className = "compra";
        document.getElementById('TxtAsientosReservado').value = nuevoasiento;
        seleccionaasientosmasivosreservado(document.getElementById("TxtAsientosReservado").value);
    }
}
//FUNCION PARA REMOVER UN ASIENTO SI YA HA SIDO SELECCIONADO O ESCRITO BOLETO_REGRESO
function removerasientoreservado(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientosReservado').value;
    var asiento = asientos.split(",");
    var nuevosasientos = '';
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] != nuevoasiento) {
            if (i == 0) {
                nuevosasientos = asiento[i];
            }
            else {
                nuevosasientos = nuevosasientos + ',' + asiento[i];
            }
            existe = false;
        }
        i++;
    }
    if (nuevosasientos.charAt(0) == ',') {
        var largo = parseInt(nuevosasientos.length)
        nuevosasientos = nuevosasientos.substring(1, largo);
    }
    return nuevosasientos;
}
function asientoexistereservado(nuevoasiento) {
    var existe = false;
    var asientos = document.getElementById('TxtAsientosReservado').value;
    var asiento = asientos.split(",");
    var i = 0;
    while (i < asiento.length && existe == false) {
        if (asiento[i] == nuevoasiento)
            existe = true;
        else
            existe = false;
        i++;
    }
    return existe;
}
//*****************************************CAMBIA EL ESTATUS A LOS ASIENTOS RESERVADOS**********************************
function cambioestatusasientoreservado(valor) {
    if (!valor)
    { }
    else {
        var arreglo = valor.split(",");
        for (i = 0; i < arreglo.length; i++) {
            var numeroasiento = arreglo[i];
            numeroasiento = 'r' + numeroasiento;
            document.getElementById(numeroasiento).className = "reservado";
        }
    }
}
//FUNCION PARA DETERMINAR LA FECHA AL DIA SIGUIENTE, SEGUN LA FECHA SELECCIONADA
function datetomorrow(fecha) {
    var arreglo = fecha.split("/");
    var mes = arreglo[1];
    var dia = arreglo[0];
    var anio = arreglo[2];
    dia = parseInt(dia) + 1;
    dia = String(dia);
    if (parseInt(dia.length) < 2) {
        dia = "0" + dia;
    }
    if (parseInt(mes.length) < 2)
        mes = "0" + mes;
    return dia + "/" + mes + "/" + anio;
}
//CUANDO SOLO ES EL BOLETO DE IDA
function estableceencabezadoanulacion() {
    var datocompleto;
    var fecha = fechaviajeocupado;
    var origen = idorigenocupado;
    var destino = iddestinoocupado;
    var nit = document.getElementById('TxtNit').value;
    var agencia = 1;
    var usuario = "admin";
    var saldo = 0;
    var total = recargocambiohorario;
    var totalunitario = recargocambiohorario;
    var horario = idhorarioocupado;
    var ruta = idrutaocupado;
    var empresa = idempresaocupado;
    var servicio = idservicioocupado;
    var nombre = document.getElementById('TxtNombre').value;
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!origen)
        origen = 0;
    if (!destino)
        destino = 0;
    if (!nit)
        nit = "C/F";
    if (!horario)
        horario = 0;
    if (!nombre)
        nombre = "";
    var viajerof = document.getElementById('TxtViajerof').value;
    if (!viajerof) viajerof = 0;
    var origentext = origenocupado;
    var destinotext = destinoocupado;
    var horatext = horarioocupado;
    horatext = horatext.substring(0, 8);
    var pagotext = "CONTADO";
    var direccion = document.getElementById('DireFac1').value;
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento) == true) {
        descuento = 0;
    }
    var usuarioAdmin = document.getElementById("TxtUsuarioAdmin").value;
    if (usuarioAdmin == "") {
        usuarioAdmin = " "
    }
    datocompleto = fecha + '|' + origen + '|' + destino + '|' + nit + '|' + seguro + '|' + viajerof + '|' + empresa + '|' + agencia + '|' + usuario + '|' + saldo + '|' + total + '|' + horario + '|' + origentext + '|' + destinotext + '|' + horatext + '|' + nombre + '|' + pagotext + '|' + ruta + '|' + totalunitario + '|' + servicio + '|' + direccion + '|' + descuento + '|' + usuarioAdmin;
    //               0             1                2           3            4                5               6               7                8                 9            10            11                12                13              14            15             16                17                18                 19             20                   21              22
    return datocompleto;
}
//FUNCION QUE ALMACENA COMO SI FUERA EN UN VECTOR LOS DATOS DEL BOLETO_ENCABEZADO SEPARADOS POR COMA
//CUANDO SOLO ES EL BOLETO DE IDA
function estableceencabezado() {
    var datocompleto;
    var fecha = document.getElementById('TxtFecha').value;
    var origen = document.getElementById('DropOrigen').value;
    var destino = document.getElementById('DropDestino').value;
    var nit = document.getElementById('TxtNit').value;
    var agencia = 1;
    var usuario = "admin";
    var saldo = 0;
    var total = document.getElementById('totalida').innerHTML;
    var totalunitario = document.getElementById('totalunitarioida').innerHTML;
    var arrayhora = document.getElementById('DropHora').value.split("|");
    var horario = arrayhora[0];
    var ruta = arrayhora[1];
    var empresa = arrayhora[2];
    var servicio = arrayhora[3];
    var nombre = document.getElementById('TxtNombre').value;   
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!origen)
        origen = 0;
    if (!destino)
        destino = 0;
    if (!nit)
        nit = "C/F";
    if (!horario)
        horario = 0;
    if (!nombre)
        nombre = "";
    var droporigen = document.getElementById('DropOrigen');
    var dropdestino = document.getElementById('DropDestino');
    var drophora = document.getElementById('DropHora');
    var viajerof = document.getElementById('TxtViajerof').value;
    if (!viajerof) viajerof = 0;
    var origentext = droporigen.options[droporigen.selectedIndex].text;
    var destinotext = dropdestino.options[dropdestino.selectedIndex].text;
    var horatext = drophora.options[drophora.selectedIndex].text;
    horatext = horatext.substring(0, 8);
    var pagotext = "CONTADO";
    var direccion = document.getElementById('DireFac1').value;
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento)==true) {
        descuento = 0;
    }
    var usuarioAdmin = document.getElementById("TxtUsuarioAdmin").value;
    if (usuarioAdmin == "") { 
        usuarioAdmin=" "
    }
    datocompleto = fecha + '|' + origen + '|' + destino + '|' + nit + '|' + seguro + '|' + viajerof + '|' + empresa + '|' + agencia + '|' + usuario + '|' + saldo + '|' + total + '|' + horario + '|' + origentext + '|' + destinotext + '|' + horatext + '|' + nombre + '|' + pagotext + '|' + ruta + '|' + totalunitario + '|' + servicio + '|' + direccion + '|' + descuento + '|' + usuarioAdmin;
    //               0             1                2           3            4                5               6               7                8                 9            10            11                12                13              14            15             16                17                18                 19             20                   21                  22
    return datocompleto;
}
function estableceencabezadoreasignar() {
    var datocompleto;
    var fecha = document.getElementById('TxtFechaServicio').value;
    var origen = document.getElementById('DropOrigenReasignar').value;
    var destino = document.getElementById('DropDestinoReasignar').value;
    var nit = document.getElementById('TxtNit').value;
    var agencia = 1;
    var usuario = "admin";
    var saldo = 0;
    var total = document.getElementById('totalReasignar').innerHTML;
    var totalunitario = document.getElementById('precioReasignar').innerHTML;
    var arrayhora = document.getElementById('DropHoraReasignar').value.split("|");
    var horario = arrayhora[0];
    var ruta = arrayhora[1];
    var empresa = arrayhora[2];
    var servicio = arrayhora[3];
    var nombre = document.getElementById('TxtNombre').value;
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!origen)
        origen = 0;
    if (!destino)
        destino = 0;
    if (!nit)
        nit = "C/F";
    if (!horario)
        horario = 0;
    if (!nombre)
        nombre = "";
    var droporigen = document.getElementById('DropOrigenReasignar');
    var dropdestino = document.getElementById('DropDestinoReasignar');
    var drophora = document.getElementById('DropHoraReasignar');
    var viajerof = document.getElementById('TxtViajerof').value;
    if (!viajerof) viajerof = 0;
    var origentext = droporigen.options[droporigen.selectedIndex].text;
    var destinotext = dropdestino.options[dropdestino.selectedIndex].text;
    var horatext = drophora.options[drophora.selectedIndex].text;
    horatext = horatext.substring(0, 8);
    var pagotext = "CONTADO";
    var direccion = document.getElementById('DireFac1').value;
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento) == true) {
        descuento = 0;
    }
    var usuarioAdmin = document.getElementById("TxtUsuarioAdmin").value;
    if (usuarioAdmin == "") {
        usuarioAdmin = " "
    }
    var boleto = document.getElementById("TxtBoleto").value;
    datocompleto = fecha + '|' + origen + '|' + destino + '|' + nit + '|' + seguro + '|' + viajerof + '|' + empresa + '|' + agencia + '|' + usuario + '|' + saldo + '|' + total + '|' + horario + '|' + origentext + '|' + destinotext + '|' + horatext + '|' + nombre + '|' + pagotext + '|' + ruta + '|' + totalunitario + '|' + servicio + '|' + direccion + '|' + descuento + '|' + usuarioAdmin + '|' + boleto;
    //               0             1                2           3            4                5               6               7                8                 9            10            11                12                13              14            15             16                17                18                 19             20
    return datocompleto;
}
//FUNCION QUE ALMACENA COMO SI FUERA EN UN VECTOR LOS DATOS DEL BOLETO_ENCABEZADO SEPARADOS POR COMA
//CUANDO SOLO ES EL BOLETO DE IDA Y VUELTA
function estableceencabezadovuelta() {
    var datocompleto;
    var fecha = document.getElementById('TxtFechaVuelta').value;
    var origen = document.getElementById('DropOrigenVuelta').value;
    var destino = document.getElementById('DropDestinoVuelta').value;
    var nit = document.getElementById('TxtNit').value;
    var viajerof = document.getElementById('TxtViajerof').value;
    var agencia = 1;
    var usuario = "admin";
    var saldo = 0;
    var total = document.getElementById('totalvuelta').innerHTML;
    var totalunitario = document.getElementById('totalunitariovuelta').innerHTML;
    var arrayhora = document.getElementById('DropHoraVuelta').value.split("|");
    var horario = arrayhora[0];
    var ruta = arrayhora[1];
    var empresa = arrayhora[2];
    var servicio = arrayhora[3];
    var nombre = document.getElementById('TxtNombre').value;
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!origen)
        origen = 0;
    if (!destino)
        destino = 0;
    if (!nit)
        nit = "C/F";
    if (!horario)
        horario = 0;
    if (!nombre)
        nombre = "";
    var droporigen = document.getElementById('DropOrigenVuelta');
    var dropdestino = document.getElementById('DropDestinoVuelta');
    var drophora = document.getElementById('DropHoraVuelta');
    var viajerof = document.getElementById('TxtViajerof').value;
    if (!viajerof) viajerof = 0;
    var origentext = droporigen.options[droporigen.selectedIndex].text;
    var destinotext = dropdestino.options[dropdestino.selectedIndex].text;
    var horatext = drophora.options[drophora.selectedIndex].text;
    horatext = horatext.substring(0, 8);
    var pagotext = "CONTADO";
    var direccion = document.getElementById('DireFac1').value;
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento) == false) {
        descuento = 0;
    }
    datocompleto = fecha + '|' + origen + '|' + destino + '|' + nit + '|' + seguro + '|' + viajerof + '|' + empresa + '|' + agencia + '|' + usuario + '|' + saldo + '|' + total + '|' + horario + '|' + origentext + '|' + destinotext + '|' + horatext + '|' + nombre + '|' + pagotext + '|' + ruta + '|' + totalunitario + '|' + servicio + '|' + direccion + '|' + descuento;
    //               0             1                2           3            4                5               6               7                8                 9            10            11                12                13              14            15             16                  17               18                  19             20
    return datocompleto;
}
//CONCATENA DATOS RELACIONADOS CON LA FACTURACION BOLETO DE IDA
function estableceencabezadofactura() {
    var datocompleto;
    var totalfactura = document.getElementById('TxtTotalFac').value;
    if (!totalfactura) totalfactura = 0;
    var cambio = document.getElementById('TxtCambio').value;
    if (!cambio) cambio = 0;
    var cheque = document.getElementById('TxtCheque').value;
    if (!cheque) cheque = 0;
    var efectivo = document.getElementById('TxtPagoEfectivo').value;
    if (!efectivo) efectivo = 0;
    efectivo = parseFloat(efectivo) - parseFloat(cambio);
    var tarjeta = document.getElementById('TxtTotalTarjetaC').value;
    if (!tarjeta) tarjeta = 0;
    var recargo = redondea(document.getElementById('TxtRecargoFac').value);
    if (!recargo) recargo = 0;
    datocompleto = totalfactura + '|' + recargo + '|' + efectivo + '|' + tarjeta + '|' + cheque;
    //               v   0                 1                2               3            4     
    return datocompleto;
}
function estableceencabezadoreservacion() {
    var datocompleto;
    var fecha = document.getElementById('TxtFechaReservado').value;
    var droporigen = document.getElementById('DropOrigenReservado');
    var dropdestino = document.getElementById('DropDestinoReservado');
    var arrayhora = document.getElementById('DropHoraReservado').value.split("|");
    var horario = arrayhora[0];
    var ruta = arrayhora[1];
    var empresa = arrayhora[2];
    var servicio = arrayhora[3];
    var agencia = 1;
    var usuario = "admin";
    var nombre = document.getElementById('TxtNombreReservado').value;
    var total = document.getElementById('totalunitarioreservado').innerHTML;
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!droporigen.value)
        droporigen.value = 0;
    if (!dropdestino.value)
        dropdestino.value = 0;
    if (!nombre)
        nombre = "";
    var origenvalue = droporigen.value;
    var origentext = droporigen.options[droporigen.selectedIndex].text;
    var destinovalue = dropdestino.value;
    var destinotext = dropdestino.options[dropdestino.selectedIndex].text;
    var horatext = 'x';
    horatext = horatext.substring(0, 8);
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento) == false) {
        descuento = 0;
    }
    datocompleto = fecha + '|' + horario + '|' + origenvalue + '|' + destinovalue + '|' + nombre + '|' + usuario + '|' + empresa + '|' + agencia + '|' + ruta + '|' + total + '|' + servicio + '|' + descuento;
    //                    fechaviaje 1  ,  id_hora 2 ,        origen 3 ,          destino 4 ,      anombre 5 ,      usuario 6  ,    empresa  7 ,   agencia  8                 ruta 9    total 10
    return datocompleto;
}
//FUNCION QUE ALMACENA COMO SI FUERA EN UN VECTOR LOS DATOS DEL BOLETO_ENCABEZADO SEPARADOS POR COMA
//CUANDO SOLO CONFIRMACION DE BOLETOS RESERVADOS
function estableceencabezadoconfirmacion() {
    var datocompleto;
    var fecha = document.getElementById('TxtFechaR').value;
    var arrayviaje = document.getElementById('TxtViajeR').value.split("-");
    var origenarray = arrayviaje[0];
    var destinoarray = arrayviaje[1];
    var nit = "N/A";
    var empresa = 1;
    var agencia = 1;
    var usuario = "admin";
    var saldo = 0;
    var total = document.getElementById('totalreservacion').innerHTML;
    var nombre = "_____";
    var ruta = 1;
    if (!fecha)
        fecha = "00/00/0000";
    else
        fecha = formatofecha(fecha);
    if (!nit)
        nit = "N/A";
    if (!nombre)
        nombre = "__________________________";
    var droporigen = document.getElementById('DropOrigen');
    var dropdestino = document.getElementById('DropDestino');
    var drophora = document.getElementById('DropHora');
    var dropformapago = document.getElementById('DropFormaPagoR');
    var origentext = droporigen.options[droporigen.selectedIndex].text;
    var destinotext = dropdestino.options[dropdestino.selectedIndex].text;
    var horatext = drophora.options[drophora.selectedIndex].text;
    horatext = horatext.substring(0, 8);
    var pagotext = dropformapago.options[dropformapago.selectedIndex].text;
    var descuento = document.getElementById("TxtDescuentoFac").value;
    if (descuento <= 0 || isNaN(descuento) == false) {
        descuento = 0;
    }
    datocompleto = fecha + '|' + origen + '|' + destino + '|' + nit + '|' + seguro + '|' + formapago + '|' + empresa + '|' + agencia + '|' + usuario + '|' + saldo + '|' + total + '|' + horario + '|' + origentext + '|' + destinotext + '|' + horatext + '|' + nombre + '|' + pagotext + '|' + ruta;
    //               0             1                2           3            4                5               6               7                8                 9            10            11                12                13              14            15             16                17
    return datocompleto;
}
function calculafactura() {
    var subtotal = document.getElementById('TxtSubTotalFac').value;
    var total = subtotal;
    var efectivo = document.getElementById('TxtPagoEfectivo').value;
    if (!efectivo) efectivo = 0;
    var cheque = document.getElementById('TxtCheque').value;
    if (!cheque) cheque = 0;
    var tarjeta = document.getElementById('TxtTarjetaC').value;
    if (!tarjeta) tarjeta = 0;
    var recargo = document.getElementById('TxtRecargoFac').value;
    if (!recargo) recargo = 0;
    var totaltarjeta = parseFloat(tarjeta) + parseFloat(recargo);
    if (!totaltarjeta) totaltarjeta = 0;
    total = parseFloat(subtotal) + parseFloat(recargo);

    var pago = parseFloat(efectivo) + parseFloat(cheque) + parseFloat(totaltarjeta);
    if (!pago) {
        cambio = 0;
        document.getElementById('TxtSuPagoFac').value = 0;
        document.getElementById('TxtCambio').value = 0;
    }
    else {
        var cambio = parseFloat(pago) - parseFloat(total);
        if (!cambio) {
            cambio = 0;
            document.getElementById('TxtCambio').style.color = "Black";
        }
        else if (parseFloat(cambio) > 0) {
            document.getElementById('TxtCambio').style.color = "Red";
        }
        document.getElementById('TxtSuPagoFac').value = redondea(pago);
        document.getElementById('TxtTotalTarjetaC').value = redondea(totaltarjeta);
        document.getElementById('TxtCambio').value = redondea(cambio);
        document.getElementById('TxtTotalFac').value = redondea(total);
    }
}
//******************************FUNCION QUE VERIFICA SI UN NIT ES VALIDO****************************************//
function validanit(valor) {
    var nit = valor;
    if (!nit)
        return false;
    nit = nit.toUpperCase();
    if (nit == 'C/F') return true;
    else if (nit == 'N/A') return true;
    else {
        var largo = parseInt(nit.length) - 1;
        nit = nit.replace("-", "");
        var temporal = 0;
        var posicion = 0;
        while (largo > 1) {
            temporal = parseInt(temporal) + parseInt(nit.charAt(posicion)) * parseInt(largo);
            largo--;
            posicion++;
        }
        largo = parseInt(nit.length);
        var resultado = temporal % 11;
        resultado = 11 - parseInt(resultado);
        var digitoverificador = resultado == 11 ? '0' :
                               resultado == 10 ? 'K' :
                               resultado;
        return (nit.charAt(largo - 1) == digitoverificador);
    }
}
//*********************************FIN FUNCION QUE VERIFICA NIT VALIDO*****************************************//
//PAGE LOAD
function pageLoad(sender, e) {

    $('#TxtTarjetaC').blur(function () {
        var tarjeta = document.getElementById('TxtTarjetaC').value;
        if ((!tarjeta) || parseFloat(tarjeta < 0.01)) {
            document.getElementById('TxtTarjetaC').value = 0;
            document.getElementById('TxtTotalTarjetaC').value = 0;
            document.getElementById('TxtRecargoFac').value = 0;
        }
        else {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/CalculaRecargo",
                data: '{tarjeta: "' + tarjeta + '"}',
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
                async: false,
                success: resultadorecargo,
                error: erroresrecargo
            });
        }
    });
    function resultadorecargo(msg) {
        var recargo = msg.d;
        document.getElementById('TxtRecargoFac').value = recargo;
        document.getElementById('TxtTotalTarjetaC').value = parseFloat(document.getElementById('TxtTarjetaC').value) + parseFloat(recargo);
        mensajecorrecto("Recargo calculado correctamente");
        calculafactura();
    }
    function erroresrecargo(msg) {
        mensajeerrorpermanente("Error en realizar calculo del recargo");
    }
    $('#BtnReservar').click(function () {
        var valor = document.getElementById("TxtAsientosReservado").value;
        if (valor != "") {
            seleccionaasientosmasivosreservado(valor);
        }
        var asientos = document.getElementById('TxtAsientosReservado').value;
        asientos = asientos.replace(/,/g, "|");
        var fecha = document.getElementById('TxtFechaReservado').value;
        var nombre = document.getElementById('TxtNombreReservado').value;
        if (!asientos) {
            mensajeadvertencia("No ha ingresado asientos");
            document.getElementById(asientos).focus();
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado a nombre de quien estará el boleto");
        }
        else {
            document.getElementById("BtnImprimirReasginar").style.display = "none";
            document.getElementById("BtnImprimir").style.display = "";
        $.ajax({
                type: "POST",
                url: "WSboleto.asmx/permisoboletos",
                data: '{usuario: "' + document.getElementById('husuario').value + '",accion: 5}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: continuarfacturandoreserva,
                async: false,
                error: continuarfacturandoreservaerror
            });
            
        }
    });
    function continuarfacturandoreservaerror(msg) {mensajeerrorpermanente("ERROR: " + msg.responseText); }
    function continuarfacturandoreserva(msg) {

        var asientos = document.getElementById('TxtAsientosReservado').value;
        asientos = asientos.replace(/,/g, "|");

        if (msg.d == 1) {
            var valor2 = estableceencabezadoreservacion();
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ReservaBoleto",
                data: '{Asientos: "' + asientos + '", Datos: "' + valor2 + '"}',
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
                success: resultadoreservacion,
                error: erroresreservacion
            });
        } else {mensajeadvertencia("No posee los permisos para realizar esta accion"); }
        }

    function resultadoreservacion(msg) {
        cambioestatusasientoreservado(document.getElementById('TxtAsientosReservado').value)
        var resultado = msg.d;
        if (resultado.substring(0, 5) == "ERROR") {
            mensajeerrorpermanente("ERROR " + resultado);
        }
        else {
            document.getElementById('TxtAsientosReservado').value = "";
            document.getElementById('TxtNombreReservado').value = "";
            mensajecorrecto(resultado);
        }
    }
    function erroresreservacion(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    //------------------MOSTRAR PANEL DE F A C T U R A C I O N DESDE EL BOTON IMPRIMIR DE B O L E T O  D E  I D A ---------------------//
    $('#BtnFacturar').click(function () {
        var valor = document.getElementById('TxtAsientos').value;
        seleccionaasientosmasivos(valor);
        var fecha = document.getElementById('TxtFecha').value;
        valor = valor.replace(/,/g, "|");
        drophora = document.getElementById('DropHora').value;
        var validaorigen = document.getElementById('DropOrigen').value;
        var validadestino = document.getElementById('DropDestino').value;
        if (!valor) {
            mensajeadvertencia("No ha ingresado asientos");
            document.getElementById('TxtAsientos').focus();
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
            document.getElementById('TxtFecha').focus();
        }
        else if (validaorigen <= 0 && document.getElementById('DropOrigen').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un origen válido");
            document.getElementById('DropOrigen').focus();
        }
        else if (validadestino <= 0 && document.getElementById('DropDestino').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un destino válido");
            document.getElementById('DropDestino').focus();
        }
        else if (document.getElementById('DropHora').value == "0" || document.getElementById('DropHora').selectedIndex == -1) {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHora').focus();
        } else {

            document.getElementById("BtnImprimirReasginar").style.display = "none";
            document.getElementById("BtnImprimir").style.display = "";

            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/permisoboletos",
                data: '{usuario: "' + document.getElementById('husuario').value + '",accion: 2}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: continuarfacturandoida,
                async: false,
                error: continuarfacturandoidaerror
            });
            
        }
    });
    function continuarfacturandoidaerror(msg) {mensajeerrorpermanente("ERROR: " + msg.responseText); }
    function continuarfacturandoida(msg) {
        if (msg.d == 1) {
            //SI SE CUMPLEN TODAS LAS CONDICIONES ENTONCES ABRIMOS EL MODAL DE FACTURACION
            /*DETENEMOS LA FUNCION DE AUTO ACTUALIZAR*/
            mostrarmodal('facturacion-modal');
            //LE ENVIAMOS EL PARAMETRO A LBLCONTROL PARA QUE EL BOTON IMPRIMIR FACTURA HAGA LA ACCION DE BOLETOS IDA PANEL FAC
            document.getElementById('lblcontrol').innerHTML = 'ida';
            var primersubtotal = document.getElementById('totalida').innerHTML;
            var subtotalcondescuento = parseFloat(primersubtotal) - (parseFloat(primersubtotal) * parseFloat(document.getElementById('lbldescuento').innerHTML) / 100);
            var descuento = parseFloat(primersubtotal) - parseFloat(subtotalcondescuento);
            coeficiente = parseInt(parseFloat(descuento) / 5);
            var descuentoreal = coeficiente * 5;
            document.getElementById('lbldescuentototal').innerHTML = redondea(descuentoreal);
            var subtotal = parseFloat(primersubtotal) - parseFloat(descuentoreal);
            document.getElementById('TxtSubTotalFac').value = redondea(subtotal);
            document.getElementById('TxtTotalFac').value = redondea(subtotal);
            document.getElementById('TxtSuPagoFac').value = "0";
            setTimeout("document.getElementById('TxtPagoEfectivo').focus()", 250);
        }
        else {mensajeadvertencia("No posee privilegios para realizar esta accion"); }
    }
    //***************************************************BOTON QUE IMPRIME SOLO IDA O IDA Y VUELTA *****************************************************/
    $('#BtnImprimir').click(function () {
        var control = document.getElementById('lblcontrol').innerHTML;
        var descuento = document.getElementById("TxtDescuentoFac").value;
        if (descuento > 0) {
            var usuario = document.getElementById("TxtUsuarioAdmin").value;
            var pass = document.getElementById("TxtPassAdmin").value;
            if (usuario != "" && pass != "") {
                $.ajax({
                    type: "POST",
                    url: "WSboleto.asmx/DeterminaUsuario",
                    data: '{usuario: "' + usuario + '", pass: "' + pass + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) {
                        var items = msg.d;
                        if (items == "") {
                            mensajeadvertencia("Usuario ó Contraseña Incorrecta");
                            document.getElementById("TxtUsuarioAdmin").focus();
                        } else if (items == "8") {
                            if (control == "ida")
                                imprimeboletodeida();
                            else if (control == "vuelta") {
                                imprimeboletodeidayvuelta();
                            }
                            else if (control == "cambiarhorario")
                                imprimeboletocambiohorario();
                        } else {
                            mensajeadvertencia("El Usuario No Cuenta Con Los Permisos Para Autorizar Acción");
                            document.getElementById("TxtUsuarioAdmin").focus();
                        }
                    },
                    error: function (msg) {
                        mensajeerrorpermanente('Error: ' + msg.responseText);
                    }
                });
            } else {
                document.getElementById("UsuarioAdmin").style.display = "";
                document.getElementById("TxtUsuarioAdmin").focus();
            }
        } else {
            if (control == "ida")
                imprimeboletodeida();
            else if (control == "vuelta")
                imprimeboletodeidayvuelta();
            else if (control == "cambiarhorario")
                imprimeboletocambiohorario();
        }
    });
    /*
    ByVal idboleto As Integer,
                                         ByVal fecha As String, ByVal idorigen As Integer, ByVal iddestino As Integer,
                                         ByVal idhora As Integer, ByVal idruta As Integer, ByVal idempresa As Integer,
                                         ByVal idservicio As Integer, ByVal numerobutaca As Integer
    */
    $('#BtnReasignarBoleto').click(function () {

        var horarios = document.getElementById('DropHoraReasignar');
        var fecha = document.getElementById('TxtFechaServicio');
        var idorigen = document.getElementById('DropOrigenReasignar');
        var idboleto = document.getElementById('TxtBoleto');
        var iddestino = document.getElementById('DropDestinoReasignar');
        var numerobutaca = document.getElementById('TxtAsientoAsignacion');

        if (!numerobutaca.value) { mensajeadvertencia("No ha ingresado un numero de butaca"); numerobutaca.focus(); return; }
        else if (!fecha.value) { mensajeadvertencia("No ha ingresado una fecha valida"); fecha.focus(); return; }
        else if (!idboleto.value) { mensajeadvertencia("No ha ingresado un numero de boleto"); idboleto.focus(); return; }
        else if (horarios.selectedIndex < 0) { mensajeadvertencia("No ha seleccionado un horario valido"); horarios.focus(); return; }
        else if (idorigen.selectedIndex < 0) { mensajeadvertencia("No ha seleccionado un origen valido"); idorigen.focus(); return; }
        else if (iddestino.selectedIndex < 0) { mensajeadvertencia("No ha seleccionado un destino valido"); iddestino.focus(); return; }
        else {

            //alert(horarios.selectedIndex);

            var arreglohorarios = horarios.value.split("|");

            var idhora = arreglohorarios[0];
            var idruta = arreglohorarios[1];
            var idempresa = arreglohorarios[2];
            var idservicio = arreglohorarios[3];
            var observaciones = document.getElementById('txtobservaciones');

            if (!observaciones.value) { mensajeadvertencia("No ha ingresado las observaciones"); observaciones.focus(); return; }

            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/cambiarHorarioBoleto",
                data: '{idboleto: ' + idboleto.value + ',fecha: "' + formatofecha(fecha.value) + '",idorigen: ' + idorigen.value + ',iddestino: ' + iddestino.value + ',idhora: ' + idhora + ',idruta: ' + idruta + ',idempresa: ' + idempresa + ',idservicio: ' + idservicio + ',numerobutaca: ' + numerobutaca.value + ',observaciones: "' + observaciones.value + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: cambiahorarioboleto,
                async: false,
                error: cambiahorarioboletono
            });
        }
    });

    function cambiahorarioboleto(msg) {
        
        var idboleto = document.getElementById('TxtBoleto');
        var numerobutaca = document.getElementById('TxtAsientoAsignacion');
 
        if (msg.d.substring(0, 5) == 'ERROR')
            mensajeerrorpermanente(msg.d);
        else {
            mensajecorrecto(msg.d);
            var asiento = document.getElementById('TxtAsientoAsignacion');
            document.getElementById('reasignar' + asiento.value).className = "ocupado";
            numerobutaca.value = "";
            idboleto.value = "";
            idboleto.focus();
        }
    }

    function cambiahorarioboletono(msg) {mensajeerror(msg.responseText); }

    /*$('#BtnReasignarBoleto').click(function () {

        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/permisoboletos",
            data: '{usuario: "' + document.getElementById('husuario').value + '",accion: 6}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: continuarfacturandoreasigna,
            async: false,
            error: continuarfacturandoreasignaerror
        });
    });*/        

    function continuarfacturandoreasignaerror(msg) {mensajeerrorpermanente("ERROR: " + msg.responseText); }
    function continuarfacturandoreasigna(msg) {
    if (msg.d==1){
        var valor = document.getElementById("TxtAsientoAsignacion").value;
        if (valor != "") {
            seleccionaasientosmasivosreasginar(valor);
        }
        var boleto = document.getElementById("TxtBoleto").value;
        if (boleto == "" || isNaN(boleto) == true) {
            mensajeadvertencia("ERROR: Boleto Incorrecto");
            return;
        }
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/verificarBoleto",
            data: '{boleto: "' + boleto + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var items = data.d;
                $.each(items, function (index, item) {
                    if (item.estado == "ERROR") {
                        mensajeadvertencia("Error: El boleto ya ha sido asignado a una butaca");
                    } else {
                        mostrarmodal('facturacion-modal');
                        //LE ENVIAMOS EL PARAMETRO A LBLCONTROL PARA QUE EL BOTON IMPRIMIR FACTURA HAGA LA ACCION DE BOLETOS IDA PANEL FAC
                        document.getElementById('lblcontrol').innerHTML = 'ida';
                        var precioActual = parseFloat(document.getElementById("totalReasignar").innerHTML);
                        var precioBoleto = parseFloat(item.precio);
                        if (precioActual > precioBoleto) {
                            var extra = precioActual - precioBoleto;
                            var primersubtotal = 10 + extra;
                            document.getElementById("TxtDiferenciaBoletos").value = extra;
                        } else {
                            var primersubtotal = 10;
                            document.getElementById("TxtDiferenciaBoletos").value = 0;
                        }
                        document.getElementById("BtnImprimirReasginar").style.display = "";
                        document.getElementById("BtnImprimir").style.display = "none";
                        var subtotalcondescuento = parseFloat(primersubtotal) - (parseFloat(primersubtotal) * parseFloat(document.getElementById('lbldescuento').innerHTML) / 100);
                        var descuento = parseFloat(primersubtotal) - parseFloat(subtotalcondescuento);
                        coeficiente = parseInt(parseFloat(descuento) / 5);
                        var descuentoreal = coeficiente * 5;
                        document.getElementById('lbldescuentototal').innerHTML = redondea(descuentoreal);
                        var subtotal = parseFloat(primersubtotal) - parseFloat(descuentoreal);
                        document.getElementById('TxtSubTotalFac').value = redondea(subtotal);
                        document.getElementById('TxtTotalFac').value = redondea(subtotal);
                        document.getElementById('TxtSuPagoFac').value = "0";
                        setTimeout("document.getElementById('TxtPagoEfectivo').focus()", 250);
                    }
                })
            },
            error: function (data) {
                mensajeadvertencia(data.d)
            }
        });
        }
        else { mensajeadvertencia("No posee los privilegios para realizar esta accion"); }
}


    document.getElementById("BtnImprimirReasginar").style.display = "none";
    document.getElementById("BtnImprimir").style.display = "";

    $("#BtnImprimirReasginar").click(function () {
        var control = document.getElementById('lblcontrol').innerHTML;
        var descuento = document.getElementById("TxtDescuentoFac").value;
        if (descuento > 0) {
            var usuario = document.getElementById("TxtUsuarioAdmin").value;
            var pass = document.getElementById("TxtPassAdmin").value;
            if (usuario != "" && pass != "") {
                $.ajax({
                    type: "POST",
                    url: "WSboleto.asmx/DeterminaUsuario",
                    data: '{usuario: "' + usuario + '", pass: "' + pass + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) {
                        var items = msg.d;
                        if (items == "") {
                            mensajeadvertencia("Usuario ó Contraseña Incorrecta");
                            document.getElementById("TxtUsuarioAdmin").focus();
                        } else if (items == "8") {
                            var cambio = document.getElementById("TxtCambio").value;
                            var pago = document.getElementById("TxtPagoEfectivo").value;
                            if (cambio >= 0 && pago > 0) {
                                imprimeboletoreasignar();
                            } else {
                                mensajeadvertencia("El Pago no es correcto");
                            }
                        } else {
                            mensajeadvertencia("El Usuario No Cuenta Con Los Permisos Para Autorizar Acción");
                            document.getElementById("TxtUsuarioAdmin").focus();
                        }
                    },
                    error: function (msg) {
                        mensajeerrorpermanente('Error: ' + msg.responseText);
                    }
                });
            } else {
                document.getElementById("UsuarioAdmin").style.display = "";
                document.getElementById("TxtUsuarioAdmin").focus();
            }
        } else {
                    var cambio = document.getElementById("TxtCambio").value;
                    var pago = document.getElementById("TxtPagoEfectivo").value;
                    if (cambio >= 0 && pago>0) {           
                        imprimeboletoreasignar();
                    } else {
                        mensajeadvertencia("El Pago no es correcto");
                    }
        }
    })
    /*NUEVA FUNCION PARA QUE ALMACENE LA NUEVA BUTACA SELECCIONADA*/
    function imprimeboletocambiohorario() {
        var asientonuevo = butacaocupadocrear;
        var fecha = fechaviajeocupado;
        var nit = document.getElementById('TxtNit').value;
        var nombre = document.getElementById('TxtNombre').value;
        var totalfactura = document.getElementById('TxtTotalFac').value;
        var pagocliente = document.getElementById('TxtSuPagoFac').value;
        var descuento = document.getElementById('TxtDescuentoFac').value;
        if (!pagocliente) pagocliente = 0;
        if (!idboletoanular) {
            mensajeadvertencia("No se ha seleccionado un boleto para anular");
            document.getElementById('TxtNit').focus();
        } else if (validanit(nit) == false) {
            mensajeadvertencia("Nit inválido");
            document.getElementById('TxtNit').focus();
        }
        else if (!nit) {
            mensajeadvertencia("No ha ingresado nit");
            document.getElementById('TxtNit').focus();
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado nombre del cliente");
            document.getElementById('TxtNombre').focus();
        }
        else if (totalfactura <= 0) {
            mensajeadvertencia("Total inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        }
        else if (parseFloat(pagocliente) < parseFloat(totalfactura)) {
            mensajeadvertencia("Pago inválido");
            document.getElementById('TxtPagoEfectivo').focus();
            
        } else if (isNaN(descuento) == true) {
            mensajeadvertencia("Descuento Invalido");
        }
        else{
            var valor2 = estableceencabezadoanulacion();
            var facturacion = estableceencabezadofactura();
            $.ajax({
                type: "POST",
                url: "WSboletotemporal.asmx/GuardaBoletoyAnula",
                data: '{asientonuevo: "' + asientonuevo + '",boletoanular: "' + idboletoanular + '", Datos: "' + valor2 + '",DatosFactura: "' + facturacion + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: resultadoboleto,
                error: erroresboleto
            });
        }
        var dropbutaca = document.getElementById('dropbutacao');
    }
    /*FIN IMPRESION BOLETO QUE HA CAMBIADO DE HORARIO*/
    function imprimeboletodeida() {
        var valor = document.getElementById('TxtAsientos').value;
        var fecha = document.getElementById('TxtFecha').value;
        var nit = document.getElementById('TxtNit').value;
        var nombre = document.getElementById('TxtNombre').value;
        var totalfactura = document.getElementById('TxtTotalFac').value;
        var pagocliente = document.getElementById('TxtSuPagoFac').value;
        var drophora = document.getElementById('DropHora').value;
        var descuento = document.getElementById("TxtDescuentoFac").value;
        if (!pagocliente) pagocliente = 0;
        valor = valor.replace(/,/g, "|");
        if (!valor) {
            mensajeadvertencia("No ha ingresado asientos");
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
        }
        else if (validanit(nit) == false) {
            mensajeadvertencia("Nit inválido");
            document.getElementById('TxtNit').focus();
        }
        else if (!nit) {
            mensajeadvertencia("No ha ingresado nit");
            document.getElementById('TxtNit').focus();
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado nombre del cliente");
            document.getElementById('TxtNombre').focus();
        }
        else if (totalfactura <= 0) {
            mensajeadvertencia("Total inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        }
        else if (parseFloat(pagocliente) < parseFloat(totalfactura)) {
            mensajeadvertencia("Pago inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        
        } else if (drophora == "0") {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHora').focus();
        } else if (isNaN(descuento)==true) {
            mensajeadvertencia("Descuento Invalido");
        } else {
            var valor2 = estableceencabezado();
            var facturacion = estableceencabezadofactura();
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/GuardaBoleto",
                data: '{Asientos: "' + valor + '", Datos: "' + valor2 + '",DatosFactura: "' + facturacion + '", Recargos:"0"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: resultadoboleto,
                error: erroresboleto
            });
        }
    }
    function resultadoboleto(msg) {
        var resultado = msg.d;
        if (resultado.substring(0, 5) == "ERROR") {
            //SUCEDIO UN ERROR ERROR.Duplicate entry '2012-12-20-1-22-5' for key 'UNIQUE'
            var cadena = resultado.split(" ");
            if (cadena[5] == "'UNIQUE_BOLETO'") {
                mensajeadvertencia("ALGUNA DE LAS BUTACAS YA SE HA VENDIDO, SELECCIONAR OTRA");
            } else if (cadena[5] == "'PRIMARY'") {
                mensajeadvertencia("FACTURA ELECTRONICA NO AUTORIZADA, FAVOR INTENTAR DE NUEVO");
            } else {
                mensajeerrorpermanente("SUCEDIO UN ERROR " + resultado);
            }
        }
        else {
            window.location = msg.d;
            if (document.getElementById("TxtAsientosR").value != "") {
                verificarReservacion();
            }
            
            cambioestatusasiento(document.getElementById('TxtAsientos').value);
            limpiarboletoida();
            document.getElementById('DropHora').focus();
            document.getElementById('TxtCambio').style.color = "Black";
            setTimeout('mensajecorrecto("Boleto guardado correctamente")', 1000);
            
            cerrarmodal('facturacion-modal');
            
            document.getElementById("totalunitarioida").innerHTML="0.00";
            document.getElementById("totalida").innerHTML="0.00";
        }
    }
    function verificarReservacion() {
        var asientos = document.getElementById("TxtAsientos").value;
        var valores = document.getElementById("DropHora").value;
        var fecha = document.getElementById("TxtFecha").value
        var butacas = document.getElementById("butacasR").value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/verificarReservacion",
            data: '{Asientos: "' + asientos + '", valores:"' + valores + '", fecha:"' + fecha + '", butacas:"' + butacas + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false
        });
    }
    function erroresboleto(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    function imprimeboletoreasignar() {
        var valor = document.getElementById('TxtAsientoAsignacion').value;
        var fecha = document.getElementById('TxtFechaServicio').value;
        var nit = document.getElementById('TxtNit').value;
        var nombre = document.getElementById('TxtNombre').value;
        var totalfactura = document.getElementById('TxtTotalFac').value;
        var pagocliente = document.getElementById('TxtSuPagoFac').value;
        var drophora = document.getElementById('DropHoraReasignar').value;
        var descuento = document.getElementById("TxtDescuentoFac").value;
        if (!pagocliente) pagocliente = 0;
        valor = valor.replace(/,/g, "|");
        if (!valor) {
            mensajeadvertencia("No ha ingresado asientos");
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
        }
        else if (validanit(nit) == false) {
            mensajeadvertencia("Nit inválido");
            document.getElementById('TxtNit').focus();
        }
        else if (!nit) {
            mensajeadvertencia("No ha ingresado nit");
            document.getElementById('TxtNit').focus();
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado nombre del cliente");
            document.getElementById('TxtNombre').focus();
        }
        else if (totalfactura <= 0) {
            mensajeadvertencia("Total inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        }
        else if (parseFloat(pagocliente) < parseFloat(totalfactura)) {
            mensajeadvertencia("Pago inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        
        } else if (drophora == "0") {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHoraReasignar').focus();
        } else if (isNaN(descuento) == true) {
            mensajeadvertencia("Descuento Invalido");
        } else {
            var extra = parseFloat(document.getElementById("TxtDiferenciaBoletos").value);
            var pago = parseFloat(document.getElementById("TxtTotalFac").value);
            var NoBoleto = document.getElementById("TxtBoleto").value;
            var recargos = extra + "," + pago + "," + NoBoleto;
            var valor2 = estableceencabezadoreasignar();
            var facturacion = estableceencabezadofactura();
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/GuardaBoleto",
                data: '{Asientos: "' + valor + '", Datos: "' + valor2 + '",DatosFactura: "' + facturacion + '", Recargos:"' + recargos + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                    var resultado = msg.d;
                    if (resultado.substring(0, 5) == "ERROR") {
                        mensajeerrorpermanente(msg.d);
                    } else {
                        $('#TxtSubTotalFac').attr('disabled', '-1');
                        document.getElementById("BtnImprimirReasginar").style.display = "none";
                        document.getElementById("BtnImprimir").style.display = "";
                        document.getElementById("TxtBoleto").value = "";
                        cerrarmodal('facturacion-modal');
                        window.location = msg.d;
                        mensajecorrecto("Boleto ReAsignado Correctamente");
                    }
                },
                error: function (msg) {
                    mensajeerrorpermanente("Error: " + msg.responseText);
                }
            });
        }
    }
    //******************************* L I M P I E Z A  D E  T E X T O S **********************************************// 
    function limpiarboletoida() {
        document.getElementById('TxtAsientos').value = "";
        document.getElementById('TxtNit').value = "N/A";
        document.getElementById('TxtNombre').value = "________________________";
        document.getElementById('TxtSuPagoFac').value = 0;
        document.getElementById('TxtCambio').value = 0;
        document.getElementById('TxtTarjetaC').value = 0;
        document.getElementById('TxtRecargoFac').value = 0;
        document.getElementById('TxtTotalTarjetaC').value = 0;
        document.getElementById('TxtPagoEfectivo').value = 0;
        document.getElementById('TxtCheque').value = 0;

    }
    function limpiarboletovuelta() {
        document.getElementById('TxtAsientosVuelta').value = "";
    }
    //----------------------------------------------------------------------------------------------------//
    //FUNCION PARA EL BOTON IMPRIMIR PERO DEL BOLETO DE VUELTA
    $('#BtnImprimirVuelta').click(function () {
        var valor = document.getElementById('TxtAsientosVuelta').value;
        var fecha = document.getElementById('TxtFechaVuelta').value;
        valor = valor.replace(/,/g, "|");
        drophora = document.getElementById('DropHoraVuelta').value;
        var validaorigen = document.getElementById('DropOrigenVuelta').value;
        var validadestino = document.getElementById('DropDestinoVuelta').value;
        if (!valor) {
            mensajeadvertencia("No ha ingresado asientos");
            document.getElementById('TxtAsientos').focus();
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
            document.getElementById('TxtFecha').focus();
        }
        else if (validaorigen <= 0 && document.getElementById('DropOrigenVuelta').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un origen");
            document.getElementById('DropOrigenVeulta').focus();
        }
        else if (validadestino <= 0 && document.getElementById('DropDestinoVuelta').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un destino");
            document.getElementById('DropDestinoVuelta').focus();
        }
        else if (drophora <= 0 && document.getElementById('DropHoraVuelta').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHoraVuelta').focus();
        } else if (drophora <= 0 && document.getElementById('DropHoraVuelta').selectedIndex != -1) {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHoraVuelta').focus();
        } else {

            document.getElementById("BtnImprimirReasginar").style.display = "none";
            document.getElementById("BtnImprimir").style.display = "";

           $.ajax({
                type: "POST",
                url: "WSboleto.asmx/permisoboletos",
                data: '{usuario: "' + document.getElementById('husuario').value + '",accion: 2}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: continuarfacturandoidayvuelta,
                async: false,
                error: continuarfacturandoidayvueltaerror
            });
            
        }
    });
    function continuarfacturandoidayvueltaerror(msg) {mensajeerrorpermanente("ERROR: " + msg.responseText); }
    function continuarfacturandoidayvuelta(msg) {
        if (msg.d == 1) {
            //SI SE CUMPLEN TODAS LAS CONDICIONES ENTONCES ABRIMOS EL MODAL DE FACTURACION
            mostrarmodal('facturacion-modal');
            //LE ENVIAMOS EL PARAMETRO A LBLCONTROL PARA QUE EL BOTON IMPRIMIR FACTURA HAGA LA ACCION DE BOLETOS IDA Y VUELTA PANEL FAC
            document.getElementById('lblcontrol').innerHTML = 'vuelta';
            var primersubtotal = redondea(parseFloat(document.getElementById('totalida').innerHTML) + parseFloat(document.getElementById('totalvuelta').innerHTML));
            //var primersubtotal = document.getElementById('totalida').innerHTML;
            var subtotalcondescuento = parseFloat(primersubtotal) - (parseFloat(primersubtotal) * parseFloat(document.getElementById('lbldescuento').innerHTML) / 100);
            var descuento = parseFloat(primersubtotal) - parseFloat(subtotalcondescuento);
            coeficiente = parseInt(parseFloat(descuento) / 5);
            var descuentoreal = coeficiente * 5;
            document.getElementById('lbldescuentototal').innerHTML = redondea(descuentoreal);
            var subtotal = parseFloat(primersubtotal) - parseFloat(descuentoreal);
            document.getElementById('TxtSubTotalFac').value = redondea(subtotal);
            document.getElementById('TxtTotalFac').value = redondea(subtotal);
            document.getElementById('TxtSuPagoFac').value = "0";
            setTimeout("document.getElementById('TxtPagoEfectivo').focus()", 250);
        } else {mensajeadvertencia("No posee los permisos para realizar esta accion"); }
        }

    //----------------------------------------------------------------------------------------------------------------------------//
    function imprimeboletodeidayvuelta() {
        var valor = document.getElementById('TxtAsientos').value;
        var valor2 = document.getElementById('TxtAsientosVuelta').value;
        var fechavuelta = document.getElementById('TxtFechaVuelta').value;
        var nit = document.getElementById('TxtNit').value;
        var nombre = document.getElementById('TxtNombre').value;
        var totalfactura = document.getElementById('TxtTotalFac').value;
        var pagocliente = document.getElementById('TxtSuPagoFac').value;
        var descuento = document.getElementById('TxtDescuentoFac').value;
        valor = valor.replace(/,/g, "|");
        valor2 = valor2.replace(/,/g, "|");
        if (!valor2) {
            mensajeadvertencia("No ha ingresado asientos");
        }
        else if (!fechavuelta) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
        }
        else if (!nit) {
            mensajeadvertencia("No ha ingresado nit");
            document.getElementById('TxtNit').focus();
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado nombre del cliente");
            document.getElementById('TxtNombre').focus();
        }
        else if (totalfactura <= 0) {
            mensajeadvertencia("Total inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        }
        else if (parseFloat(pagocliente) < parseFloat(totalfactura)) {
            mensajeadvertencia("Pago inválido");
            document.getElementById('TxtPagoEfectivo').focus();
        
        }
        else if (drophora == "0") {
            mensajeadvertencia("Seleccione un horario válido");
            document.getElementById('DropHora').focus();
        } else if (isNaN(descuento)==true) {
            mensajeadvertencia("Descuento Invalido");
        } else {
            var valor3 = estableceencabezado();
            var valor4 = estableceencabezadovuelta();
            var valor5 = estableceencabezadofactura();
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/GuardaBoletoIdaVuelta",
                data: '{AsientosIda: "' + valor + '",AsientosVuelta: "' + valor2 + '", DatosIda: "' + valor3 + '", DatosVuelta: "' + valor4 + '", DatosFactura: "' + valor5 + '", Recargos:"0"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: resultadoboletovuelta,
                error: erroresboletovuelta
            });
        }
    }
    function resultadoboletovuelta(msg) {
        var resultado = msg.d;
        if (resultado.substring(0, 5) == "ERROR") {
            mensajeerrorpermanente("SUCEDIO UN ERROR " + resultado);
        }
        else {
            $('.tab-content').animate({ backgroundColor: "#FFF" }, 1000);
            window.location = msg.d;
            cambioestatusasiento(document.getElementById('TxtAsientos').value)
            cambioestatusasientovuelta(document.getElementById('TxtAsientosVuelta').value)
            mensajecorrecto("Boletos guardados correctamente");
            limpiarboletoida();
            limpiarboletovuelta();
            /*$('.tab-content div.tab').slideUp('slow');
            $('.contenedor div.contenedorind').slideUp('slow');
            $('.tab-content div.tab:eq(0)').slideDown('slow');
            $('.contenedor div.contenedorind:eq(0)').slideDown('slow');*/
            
            document.getElementById('fieldsetida').style.display = '';
            document.getElementById('contenedorbus').style.display = '';
            document.getElementById('fieldsetretorno').style.display = 'none';
            document.getElementById('contenedorbusvuelta').style.display = 'none';
            
            setTimeout("document.getElementById('DropHora').focus()", 1500);
            cerrarmodal('facturacion-modal');
            setTimeout('mensajecorrecto("Boleto guardado correctamente")', 1000);
        }
    }
    function erroresboletovuelta(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    //BOTON PARA CONFIRMAR LOS ASIENTOS RESERVADOS.....
    $('#BtnConfirmarReservado').click(function () {
        var validachecks = $(".chks:checked").length;
        var valor2 = 1;
        if (validachecks <= 0) {
            mensajeadvertencia("No ha seleccionado por lo menos un asiento");
        }
        else {
            document.getElementById("TxtAsientos").value = document.getElementById("TxtAsientosR").value;
            var primersubtotal = document.getElementById('totalreservacion').innerHTML;
            var butacas = new Array;
            var inputs = document.getElementsByTagName('input');
            var conter = 0;
            for (var i = 0, j = inputs.length; i < j; i++) {
                var input = inputs[i];
                if (input.type && input.type === 'checkbox') {
                    var id = (input.id).split("-");
                    butacas[conter] = id[1];
                    conter++;
                }
            }
            setTimeout("mostrarmodal('facturacion-modal');", 50);
            
            document.getElementById('lblcontrol').innerHTML = 'ida';
            var subtotalcondescuento = parseFloat(primersubtotal) - (parseFloat(primersubtotal) * parseFloat(document.getElementById('lbldescuento').innerHTML) / 100);
            var descuento = parseFloat(primersubtotal) - parseFloat(subtotalcondescuento);
            coeficiente = parseInt(parseFloat(descuento) / 5);
            var descuentoreal = coeficiente * 5;
            document.getElementById("butacasR").value = butacas;
            document.getElementById('lbldescuentototal').innerHTML = redondea(descuentoreal);
            var subtotal = parseFloat(primersubtotal) - parseFloat(descuentoreal);
            document.getElementById('TxtSubTotalFac').value = redondea(subtotal);
            document.getElementById('TxtTotalFac').value = redondea(subtotal);
            document.getElementById('TxtSuPagoFac').value = "0";
            setTimeout("document.getElementById('TxtPagoEfectivo').focus()", 250);
        }
    });
    function resultadoconfirmacion(msg) {
        ventanapopup = window.open(msg.d, "miventana", "width=0,height=0,menubar=no");
        setTimeout("ventanapopup.close()", 2000);
    }
    function erroresconfirmacion(msg) {
        mensajeadvertencia('Error: ' + msg.responseText);
    }
    $("#BtnAnularReservado").click(function () {
        var validachecks = $(".chks:checked").length;
        var valor2 = 1;
        var butacas = "";
        if (validachecks <= 0) {
            mensajeadvertencia("No ha seleccionado por lo menos un asiento");
        }
        else {
            var inputs = document.getElementsByTagName('input');
            var counter = 0;
            for (var i = 0, j = inputs.length; i < j; i++) {
                var input = inputs[i];
                if (input.type && input.type === 'checkbox' && input.checked==true) {
                    var butaca = input.id.split("-")
                    if (counter == 0) {
                        butacas = butaca[1];
                    } else {
                        butacas = butacas + "," + butaca[1];
                    }
                    counter++;
                }
            }
            var fecha = document.getElementById("TxtFecha").value;
            var valores = document.getElementById("DropHora").value;
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/anularReservacion",
                data: '{Asientos: "' + butacas + '",valores: "' + valores + '", fecha: "' + fecha + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    var mibutaca = butacas.split(",");
                    var i = 0;
                    while (i <= mibutaca.length-1) {
                        document.getElementById(mibutaca[i]).className = "libre";
                        document.getElementById(mibutaca[i]).title = "";
                        i++;
                    }
                    cerrarmodal('facturacion-modal');
                    mensajecorrecto("Butaca(s) han sido anuladas");
                },
                error: function (data) {
                    mensajeerrorpermanente("error: " + data.responseText);
                }
            });
        }
    });
    //********************FUNCION PARA MOSTRAR EL NOMBRE DEL CLIENTE DEL NIT INGRESADO****************************//
    $('#TxtNit').change(function () {
        var valor = $('#TxtNit').val();
        valor = valor.toUpperCase();
        if (valor == "C/F") {
            document.getElementById('TxtNombre').value = "CONSUMIDOR FINAL";
            document.getElementById('DireFac1').value = "__________________________";
            document.getElementById('TxtTarjetaC').focus();
        }
        else if (valor == "N/A") {
            document.getElementById('TxtNombre').value = "___________________________";
            document.getElementById('DireFac1').value = "__________________________";
            document.getElementById('TxtTarjetaC').focus();
        }
        else if (validanit(valor) == false) {
            mensajeadvertencia("NIT inválido");
            document.getElementById('TxtNit').focus();
        }
        else {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/BuscaNit",
                data: '{Nit: "' + valor + '"}',
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
                success: resultadonit,
                async: false,
                error: erroresnit
            });
        }
    });
    function resultadonit(msg) {
        var index = 0;
        $.each(msg.d, function () {
            document.getElementById('TxtNombre').value = this.nombrecliente;
            document.getElementById('DireFac1').value = this.direccioncliente;
            index++;
        });
    }
    function erroresnit(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    //***********************PROCEDIMIENTO PARA EL DROP DE ORIGEN Y DESTINO************************************//
    $('#DropOrigen').bind("blur", function () {
        cargarTurno();
    });
    $('#DropOrigenReasignar').bind("blur", function () {
        cargarTurnoReasignar();
    });
    $('#DropOrigenReservado').bind("blur", function () {
        cargarTurnoReserva();
    });
    $('#DropOrigenVuelta').bind("blur", function () {
        cargarTurnoVuelta();
    });
    //************************PROCEDIMIENTO PARA EL DROP DE ORIGEN Y DESTINO PERO DE VUELTA****************************/
    $('#DropDestinoVuelta').bind("blur", function () {
        muestratotalVuelta();
    });
    
    //************************************FIN DROP DESTINO RESERVADO *************************************************//
    //************************FUNCION PARA MOSTRAR EL DISEÑO DEL BUS O DEL TIPO DE BUS ASIGNADO A ESA HORA*************************//
    $("#DropDestino").bind('blur', function () {
        muestratotal();

        var textoasientos = document.getElementById('TxtAsientos');
        if (textoasientos.value != "") {
            //setTimeout("seleccionaasientosmasivos(textoasientos.value);", 500); 
            seleccionaasientosmasivos(textoasientos.value); 
        }

    });

    $('#DropHora').bind("blur", function () {
        $("#contenedorbus").fadeOut(250);
        var fecha = formatofecha(document.getElementById('TxtFecha').value);
        var arrayhora = document.getElementById('DropHora').value.split("|");
        var horario = arrayhora[0];
        var ruta = arrayhora[1];
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        //SOLO REALIZAMOS EL LLAMADO AL WEBSERVICE SI SE SELECCIONA UN HORARIO VALIDO
        if (document.getElementById('DropHora').value != "0" && document.getElementById('DropHora').selectedIndex != -1) {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ExtraeHtml",
                data: '{Horario: "' + horario + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Campo: "' + 1 + '",Fecha: "' + fecha + '",Servicio: "' + servicio + '"}',
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar").removeClass("mostrar");
                    $("#contenedorbus").fadeIn(500);
                    $("#cargar").addClass("ocultar");
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resultado,
                error: errores
            });
        }
    });

    function errores(msg) {
        $("#contenedorbus").fadeIn(1000);
        mensajeerrorpermanente("ERROR: " + msg.responseText);
    }

    function resultado(msg) {
        //MOSTRAMOS EL CODIGO HTML AL BUS O AL SERVICIO CORRESPONDIENTE
        document.getElementById('contenedorbus').innerHTML = msg.d;
        //CALCULO DEL TOTAL POR SI NO CAMBIO EL RESULTADO EN EL EVENTO CHANGE
        muestratotal();
        var arrayhora = document.getElementById('DropHora').value.split("|");
        var fecha = document.getElementById('TxtFecha').value;
        fecha = formatofecha(fecha);
        var varorigen = document.getElementById('DropOrigen').value;
        var vardestino = document.getElementById('DropDestino').value;
        //LLAMADO A WEBSERVICE PARA DETERMINAR LOS ASIENTOS QUE ESTAN OCUPADOS//
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/DeterminaAsientos",
            data: '{Fecha: "' + fecha + '",Hora: "' + arrayhora[0] + '",Ruta: "' + arrayhora[1] + '",Empresa: "' + arrayhora[2] + '",Origen: "' + varorigen + '",Destino: "' + vardestino + '",Servicio: "' + arrayhora[3] + '"}',
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
            success: resultadoasientos,
            error: erroresasientos
        });
        function erroresasientos(msg) {
            mensajeerrorpermanente('Error: ' + msg.responseText);
        }
        function resultadoasientos(msg) {
            asientosdeida = msg.d;
            var ocupados = 0;
            var reservados = 0;
            var libres = 0;
            $.each(asientosdeida, function (index, asiento) {

                try {
                    //mensajecorrecto(asiento.asientonumero);
                    if (asiento == 0) {
                        mensajeadvertencia(asiento);
                    } else {
                        var nuevoasiento = document.getElementById(asiento.asientonumero);
                        nuevoasiento.className = asiento.asientoestado;
                        nuevoasiento.title = asiento.cliente + "|DESTINO: " + asiento.viaje + "|TOTAL: " + asiento.total;
                        if (asiento.asientoestado == "ocupado") {
                            nuevoasiento.className = "ocupado";
                            ocupados++;
                        }
                        else
                            if (asiento.asientoestado == "reservado") {
                                nuevoasiento.className = "reservado";
                                reservados++;
                            }
                    }
                    } catch (ex) {
                    
                    if (asiento.asientoestado == "ocupado") ocupados++;
                }
            });
            /*MOSTRAMOS EL NUMERO DE ASIENTOS OCUPADOS,RESERVADOS Y LIBRES*/
            document.getElementById('thocupado').innerHTML = ocupados;
            document.getElementById('threservado').innerHTML = reservados;
            var contadorlibres = 0;
            $("#contenedorbus .libre").each(function (index) {
                contadorlibres++;
            })
            document.getElementById('thlibre').innerHTML = contadorlibres;
            var valor = document.getElementById("TxtAsientos").value;
            if (valor != "") {
                seleccionaasientosmasivos(valor);
            }
        }
    }
    $('#DropDestinoReasignar').bind('blur', function () {
        muestratotalReasig();
    })
    $('#DropHoraReasignar').bind("blur", function () {
        $("#contenedorbus_reasignacion").fadeOut(250);
        var fecha = formatofecha(document.getElementById('TxtFechaServicio').value);
        var arrayhora = document.getElementById('DropHoraReasignar').value.split("|");
        var horario = arrayhora[0];
        var ruta = arrayhora[1];
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        //SOLO REALIZAMOS EL LLAMADO AL WEBSERVICE SI SE SELECCIONA UN HORARIO VALIDO
        if (document.getElementById('DropHoraReasignar').value != "0" && document.getElementById('DropHoraReasignar').selectedIndex != -1) {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ExtraeHtml",
                data: '{Horario: "' + horario + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Campo: "' + 4 + '",Fecha: "' + fecha + '",Servicio:"' + servicio + '"}',
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#cargar").removeClass("mostrar");
                    $("#contenedorbus_reasignacion").fadeIn(500);
                    $("#cargar").addClass("ocultar");
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resultadoReasig,
                error: erroresReasig
            });
        }
    });
 function erroresReasig(msg) {
     $("#contenedorbus_reasignacion").fadeIn(1000);
     mensajeerrorpermanente("ERROR: " + msg.responseText);
   }
   function muestratotalReasig() {
       var arrayhora = document.getElementById('DropHoraReasignar').value.split("|");
       var empresa = arrayhora[2];
       var servicio = arrayhora[3];
       var origen = document.getElementById('DropOrigenReasignar').value;
       var destino = document.getElementById('DropDestinoReasignar').value;
       $.ajax({
           type: "POST",
           url: "WSboleto.asmx/PrecioBoleto",
           data: '{origen: "' + origen + '",destino: "' + destino + '",empresa: "' + empresa + '",servicio: "' + servicio + '"}',
           async: false,
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
           success: function (msg) {
               document.getElementById('precioReasignar').innerHTML = redondea(parseInt(msg.d));
           },
           error: function (msg) {
               mensajeerrorpermanente("ERROR: " + msg.responseText);
           }
       });
       var valor = document.getElementById("TxtAsientoAsignacion").value;
       if (valor != "") {
           seleccionaasientosmasivosreasginar(valor);
       }
   }
   function resultadoReasig(msg) {
       //MOSTRAMOS EL CODIGO HTML AL BUS O AL SERVICIO CORRESPONDIENTE
       document.getElementById("contenedorbus_reasignacion").innerHTML = msg.d;
        //CALCULO DEL TOTAL POR SI NO CAMBIO EL RESULTADO EN EL EVENTO CHANGE
        muestratotalReasig();
        var arrayhora = document.getElementById('DropHoraReasignar').value.split("|");
        var fecha = document.getElementById('TxtFechaServicio').value;
        fecha = formatofecha(fecha);
        var varorigen = document.getElementById('DropOrigenReasignar').value;
        var vardestino = document.getElementById('DropDestinoReasignar').value;
        //LLAMADO A WEBSERVICE PARA DETERMINAR LOS ASIENTOS QUE ESTAN OCUPADOS//
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/DeterminaAsientos",
            data: '{Fecha: "' + fecha + '",Hora: "' + arrayhora[0] + '",Ruta: "' + arrayhora[1] + '",Empresa: "' + arrayhora[2] + '",Origen: "' + varorigen + '",Destino: "' + vardestino + '",Servicio: "' + arrayhora[3] + '"}',
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
            success: resultadoasientosReasig,
            error: erroresasientosReasig
        });
        function erroresasientosReasig(msg) {
            mensajeerrorpermanente('Error: ' + msg.responseText);
      }
      function resultadoasientosReasig(msg) {
            asientos = msg.d;
            var ocupados = 0;
            var reservados = 0;
            var libres = 0;
            $.each(asientos, function (index, asiento) {
                var nuevoasiento = document.getElementById("reasignar" + asiento.asientonumero);
                nuevoasiento.className = asiento.asientoestado;
                nuevoasiento.title = asiento.cliente + "|DESTINO: " + asiento.viaje + "|TOTAL: " + asiento.total;
                if (asiento.asientoestado == "ocupado") {
                    nuevoasiento.className = "ocupado";
                    nuevoasiento.disabled = true;
                    ocupados++;
                }
                else
                    if (asiento.asientoestado == "reservado") {
                        nuevoasiento.className = "reservado";
                        nuevoasiento.disabled = true;
                        reservados++;
                    }
            });
            /*MOSTRAMOS EL NUMERO DE ASIENTOS OCUPADOS,RESERVADOS Y LIBRES*/
            document.getElementById('thocupadoa').innerHTML = ocupados;
            document.getElementById('threservadoa').innerHTML = reservados;
            var contadorlibres = 0;
            $("#contenedorbus_reasignacion .libre").each(function (index) {
                contadorlibres++;
            })
            document.getElementById('thlibrea').innerHTML = contadorlibres;
        }
    }
    //**********************************FIN DROP HORA DE IDA***************************************//
    //****************DETERMINA ASIENTOS Y CARGA CODIGO HTML PERO DEL BUS DE REGRESO****************//
    $('#DropHoraVuelta').bind("blur", function () {
        $("#contenedorbusvuelta").fadeOut(250);
        var arrayhora = document.getElementById('DropHoraVuelta').value.split("|");
        var fecha = formatofecha(document.getElementById('TxtFechaVuelta').value);
        var horario = arrayhora[0];
        var ruta = arrayhora[1];
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        if (document.getElementById('DropHoraVuelta').value != "0" && document.getElementById('DropHoraVuelta').selectedIndex != -1) {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ExtraeHtml",
                data: '{Horario: "' + horario + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Campo: "' + 2 + '",Fecha: "' + fecha + '",Servicio: "' + servicio + '"}',
                async: false,
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#contenedorbusvuelta").fadeIn(500);
                    $("#cargar").removeClass("mostrar");
                    $("#cargar").addClass("ocultar");
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resultadovuelta,
                error: erroresvuelta
            });
        }
    });
    function erroresvuelta(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    function resultadovuelta(msg) {
        document.getElementById('contenedorbusvuelta').innerHTML = msg.d;
        //MOSTRAMOS EL PRECIO DEL BOLETO UNITARIO
        muestratotalvuelta();
        var fecha = document.getElementById('TxtFechaVuelta').value;
        fecha = formatofecha(fecha);
        var arrayhora = document.getElementById('DropHoraVuelta').value.split("|");
        var varorigen = document.getElementById('DropOrigenVuelta').value;
        var vardestino = document.getElementById('DropDestinoVuelta').value;

        //setTimeout('muestrabutacasocupadasVuelta(fecha,arrayhora[0],arrayhora[1],arrayhora[2],varorigen,vardestino,arrayhora[3]);', 500);

        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/DeterminaAsientos",
            data: '{Fecha: "' + fecha + '",Hora: "' + arrayhora[0] + '",Ruta: "' + arrayhora[1] + '",Empresa: "' + arrayhora[2] + '",Origen: "' + varorigen + '",Destino: "' + vardestino + '",Servicio: "' + arrayhora[3] + '"}',
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: resultadoasientosvuelta,
            errorasientos: erroresasientosvuelta
        });
        function erroresasientosvuelta(msg) {
            mensajeerrorpermanente('Error: ' + msg.responseText);
        }
        function resultadoasientosvuelta(msg) {
            var asientos = msg.d;
            var ocupados = 0;
            var reservados = 0;
            $.each(asientos, function (index, asiento) {
                var nuevoasiento = document.getElementById("Button" + asiento.asientonumero);
                nuevoasiento.className = asiento.asientoestado;
                nuevoasiento.title = asiento.cliente + "|DESTINO: " + asiento.viaje + "|TOTAL: " + asiento.total;
                if (asiento.asientoestado == "ocupado") {
                    nuevoasiento.className = "ocupado";
                    ocupados++;
                }
                else {
                    nuevoasiento.className = "reservado";
                    reservados++;
                }
            });
            /*MOSTRAMOS EL NUMERO DE ASIENTOS OCUPADOS,RESERVADOS Y LIBRES*/
            document.getElementById('thocupado').innerHTML = ocupados;
            document.getElementById('threservado').innerHTML = reservados;
            var contadorlibres = 0;
            $("#contenedorbusvuelta .libre").each(function (index) {
                contadorlibres++;
            })
            document.getElementById('thlibre').innerHTML = contadorlibres;
        }


    }
   
    //*******************************FIN DE HORA VUELTA****************************************************//
    //*******************DETERMINA ASIENTOS Y CARGA CODIGO HTML PERO DE LAS RESERVACIONES*****************//
    $('#DropDestinoReservado').bind('blur', function () {
        muestratotalreservado();
    });
    $('#DropHoraReservado').bind("blur", function () {
        $("#contenedorreservado").fadeOut(250);
        var fecha = formatofecha(document.getElementById('TxtFechaReservado').value);
        var arrayhora = document.getElementById('DropHoraReservado').value.split("|");
        var horario = arrayhora[0];
        var ruta = arrayhora[1];
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
         if (document.getElementById('DropHoraReservado').value != "0") {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ExtraeHtml",
                data: '{Horario: "' + horario + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Campo: "' + 3 + '",Fecha: "' + fecha + '",Servicio: "' + servicio + '"}',
                async: false,
                beforeSend: function () {
                    $("#cargar").removeClass("ocultar");
                    $("#cargar").addClass("mostrar");
                },
                complete: function () {
                    $("#contenedorreservado").fadeIn(500);
                    $("#cargar").removeClass("mostrar");
                    $("#cargar").addClass("ocultar");
                },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resultadoreservado,
                error: erroresreservado
            });
        }
    });
    function erroresreservado(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    function resultadoreservado(msg) {
        document.getElementById('contenedorreservado').innerHTML = msg.d;
        var hora = $('#DropHoraReservado').val();
        //MOSTRAMOS EL VALOR DEL BOLETO UNITARIO
        muestratotalreservado();
        var arrayhora = document.getElementById('DropHoraReservado').value.split("|");
        var fecha = document.getElementById('TxtFechaReservado').value;
        fecha = formatofecha(fecha);
        var varorigen = document.getElementById('DropOrigenReservado').value;
        var vardestino = document.getElementById('DropDestinoReservado').value;

        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/DeterminaAsientos",
            data: '{Fecha: "' + fecha + '",Hora: "' + arrayhora[0] + '",Ruta: "' + arrayhora[1] + '",Empresa: "' + arrayhora[2] + '",Origen: "' + varorigen + '",Destino: "' + vardestino + '",Servicio: "' + arrayhora[3] + '"}',
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
            success: resultadoasientosreservado,
            errorasientos: erroresasientosreservado
        });
        function erroresasientosreservado(msg) {
            mensajeerrorpermanente('Error: ' + msg.responseText);
        }
        function resultadoasientosreservado(msg) {
            //alert("resultado asientos reservados");
            var asientos = msg.d;
            var ocupados = 0;
            var reservados = 0;
            $.each(asientos, function (index, asiento) {
                var nuevoasiento = document.getElementById("r" + asiento.asientonumero);
                nuevoasiento.className = asiento.asientoestado;
                nuevoasiento.title = asiento.cliente + "|DESTINO: " + asiento.viaje + "|TOTAL: " + asiento.total;

                if (asiento.asientoestado == "ocupado") {
                    nuevoasiento.disabled = true;
                    nuevoasiento.className = "ocupado";
                    ocupados++;
                }
                else {
                    nuevoasiento.className = "reservado";
                    reservados++;
                }
            });

            /*MOSTRAMOS EL NUMERO DE ASIENTOS OCUPADOS,RESERVADOS Y LIBRES*/
            document.getElementById('thocupador').innerHTML = ocupados;
            document.getElementById('threservador').innerHTML = reservados;
            var contadorlibres = 0;
            $("#contenedorreservado .libre").each(function (index) {
                contadorlibres++;
            })
            document.getElementById('thlibrer').innerHTML = contadorlibres;

            var valor = document.getElementById("TxtAsientosReservado").value;
            if (valor != "") {
                seleccionaasientosmasivosreservado(valor);
            }
        }
    }
    //*******************************FIN DE HORA DE RESERVACION****************************************//
    //*********************************MOSTRAR PRECIO BOLETO DE IDA****************************//
    function muestratotal() {
        var arrayhora = document.getElementById('DropHora').value.split("|");
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        var origen = document.getElementById('DropOrigen').value;
        var destino = document.getElementById('DropDestino').value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/PrecioBoleto",
            data: '{origen: "' + origen + '",destino: "' + destino + '",empresa: "' + empresa + '",servicio: "' + servicio + '"}',
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
            async: false,
            success: resultadoprecio,
            error: erroresprecio
        });
    }
    function resultadoprecio(msg) {
        document.getElementById('totalunitarioida').innerHTML = redondea(parseInt(msg.d));
    }
    function erroresprecio(msg) {
        mensajeerrorpermanente("ERROR: " + msg.responseText);
    }

    function muestratotalVuelta() {
        var arrayhora = document.getElementById('DropHoraVuelta').value.split("|");
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        var origen = document.getElementById('DropOrigenVuelta').value;
        var destino = document.getElementById('DropDestinoVuelta').value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/PrecioBoleto",
            data: '{origen: "' + origen + '",destino: "' + destino + '",empresa: "' + empresa + '",servicio: "' + servicio + '"}',
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
            success: function(msg) {
                document.getElementById('totalunitariovuelta').innerHTML = redondea(parseInt(msg.d));
            },
            error: erroresprecio
        });
    }
    
    //***************************************FIN DE MUESTRA TOTAL DE IDA*****************************************//
    //**************************************MOSTRAR PRECIO DE VUELTA******************************************//
    function muestratotalvuelta() {
        var arrayhora = document.getElementById('DropHoraVuelta').value.split("|");
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        var origen = document.getElementById('DropOrigenVuelta').value;
        var destino = document.getElementById('DropDestinoVuelta').value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/PrecioBoleto",
            data: '{origen: "' + origen + '",destino: "' + destino + '",empresa: "' + empresa + '",servicio: "' + servicio + '"}',
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
            success: resultadopreciovuelta,
            error: errorespreciovuelta
        });
    }
    function resultadopreciovuelta(msg) {
        document.getElementById('totalunitariovuelta').innerHTML = redondea(parseInt(msg.d));
    }
    function errorespreciovuelta(msg) {
        mensajeerrorpermanente("ERROR: " + msg.responseText);
    }
    //******************************* FIN DE MOSTRAR PRECIO DE VUELTA *********************************//
    //*********************************MOSTRAR PRECIO BOLETO DE RESERVA*******************************//
    function muestratotalreservado() {
        var arrayhora = document.getElementById('DropHoraReservado').value.split("|");
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        var origen = document.getElementById('DropOrigenReservado').value;
        var destino = document.getElementById('DropDestinoReservado').value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/PrecioBoleto",
            data: '{origen: "' + origen + '",destino: "' + destino + '",empresa: "' + empresa + '",servicio: "' + servicio + '"}',
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
            success: reservaprecio,
            error: reservaerroresprecio
        });
    }
    function reservaprecio(msg) {
        document.getElementById('totalunitarioreservado').innerHTML = redondea(parseInt(msg.d));
    }
    function reservaerroresprecio(msg) {
        mensajeerrorpermanente("ERROR: " + msg.responseText);
    }
    //*************************** FIN DE MOSTRAR TOTAL DE RESERVA******************************//
    //*************PROCEDIMIENTO PARA VERIFICAR SI EL CLIENTE POSEE TARJETA DE VIAJERO FRECUENTE**************//
    $('#TxtViajerof').change(function () {
        var codigo = document.getElementById('TxtViajerof').value;
        if (!codigo) {
            document.getElementById('lbldescuento').innerHTML = "0";
            document.getElementById('TxtNit').value = "N/A";
            document.getElementById('TxtNombre').value = "______________________________";
            document.getElementById('DireFac1').value = "____________________________";
        }
        else {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/Buscaviajerofrecuente",
                data: '{codigo: "' + codigo + '"}',
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
                success: resultadoviajerofrecuente,
                error: erroresviajerofrecuente
            });
        }
    });
    function resultadoviajerofrecuente(msg) {
        $.each(msg.d, function () {
            if (this.descuentoviajero > 0) {
                document.getElementById('lbldescuento').innerHTML = redondea(this.descuentoviajero);
                document.getElementById('TxtNit').value = this.nitviajero;
                document.getElementById('TxtNombre').value = this.nombreviajero;
                document.getElementById('DireFac1').value = this.direccionviajero;
            }
        });
    }
    function erroresviajerofrecuente(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    //******************************FIN DE TARJETA VIAJERO FRECUENTE*********************************//
    /*****************************ULTIMOS EVENTOS AGREGADOS*****************************************/
    //************************FUNCION PARA MOSTRAR EL DISEÑO DEL BUS O DEL TIPO DE BUS ASIGNADO A ESA HORA*************************//
    $('#drophorarioo').bind("change blur", function () {
        var fecha = formatofecha(document.getElementById('txtfechaviajeo').value);
        var arrayhora = document.getElementById('drophorarioo').value.split("|");
        var horario = arrayhora[0];
        var ruta = arrayhora[1];
        var empresa = arrayhora[2];
        var servicio = arrayhora[3];
        //SOLO REALIZAMOS EL LLAMADO AL WEBSERVICE SI SE SELECCIONA UN HORARIO VALIDO
        if (document.getElementById('drophorarioo').value != "0" && document.getElementById('DropHora').selectedIndex != -1) {
            $.ajax({
                type: "POST",
                url: "WSboleto.asmx/ExtraeHtml",
                data: '{Horario: "' + horario + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Campo: "' + 1 + '",Fecha: "' + fecha + '",Servicio: "' + servicio + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: resultadoocupado,
                error: erroresocupado
            });
        }
    });
    function erroresocupado(msg) {
        mensajeerrorpermanente(msg.responseText);
    }
    function resultadoocupado(msg) {
        //MOSTRAMOS EL CODIGO HTML AL BUS O AL SERVICIO CORRESPONDIENTE
        document.getElementById('divbutacas').innerHTML = msg.d;
        //CALCULO DEL TOTAL POR SI NO CAMBIO EL RESULTADO EN EL EVENTO CHANGE
        var arrayhora = document.getElementById('drophorarioo').value.split("|");
        var fecha = document.getElementById('txtfechaviajeo').value;
        fecha = formatofecha(fecha);
        var varorigen = idorigenocupado;
        var vardestino = iddestinoocupado;
        //LLAMADO A WEBSERVICE PARA DETERMINAR LOS ASIENTOS QUE ESTAN OCUPADOS//
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/DeterminaAsientos",
            data: '{Fecha: "' + fecha + '",Hora: "' + arrayhora[0] + '",Ruta: "' + arrayhora[1] + '",Empresa: "' + arrayhora[2] + '",Origen: "' + varorigen + '",Destino: "' + vardestino + '",Servicio: "' + arrayhora[3] + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: resultadoasientosocupados,
            error: erroresasientosocupados
        });
    }
    function erroresasientosocupados(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    function resultadoasientosocupados(msg) {
        asientosdeida = msg.d;
        var asientos = 0;
        var strocupados = "";
        var reservados = 0;
        var strlibres = "";
        /*DETERMINAMOS CUANTOS ASIENTOS ESTAN OCUPADOS O RESERVADOS*/
        $.each(asientosdeida, function (index, asiento) {
            if (asiento.asientoestado == "ocupado" || asiento.asientoestado == "reservado")
                strocupados = strocupados + "," + asiento.asientonumero;
        });
        strocupados = strocupados.substring(1, strocupados.length);
        var arregloocupados = strocupados.split(",");
        /*SOLO EJECUTAMOS EL ALGORITMO SI EXISTENS BUTACAS OCUPADAS O RESERVADAS*/
        if (arregloocupados.length > 1) {
            $("#divbutacas .libre").each(function (index) {
                /*INICIALIZAMOS LA VARIABLE ID LA CUAL POSEE EL NUMERO DE BUTACA*/
                var id = $(this).attr("id");
                /*INICIALIZAMOS LA VARIABLE BOOLENA QUE POSIBLEMENTE CAMBIARA A FALSE SI ENCUENTRA UN ASIENTO OCUPADO*/
                var existelocal = true;
                /*LEEMOS EL ARREGLO CON LOS ASIENTOS OCUPADOS*/
                for (i = 0; i < arregloocupados.length; i++) {
                    var numeroasiento = arregloocupados[i];
                    /*SI EL ID DEL ASIENTO COINCIDE CON ALGUN ASIENTO OCUPADO*/
                    if (id == numeroasiento) {
                        existelocal = false;
                        break;
                    }
                }
                /*SI SE MANTUVO EL EXISTE==TRUE, QUIERE DECIR QUE NO EL ASIENTO NO COINCIDIO CON NINGUN ASIENTO OCUPADO*/
                if (existelocal == true)
                { asientos++; strlibres = strlibres + "," + id; }
                /*SI YA ENCONTRAMOS 5 ASIENTOS LIBRES TERMINAMOS EL CICLO*/
                if (asientos > 4) return false;
            });
        }
        else {
            /*RETORNAMOS LOS PRIMEROS 5 ASIENTOS SI EL BUS AUN NO TIENE VENDIDO NINGUN BOLETO*/
            strlibres = ",1,2,3,4,5";
        }
        strlibres = strlibres.substring(1, strlibres.length);
        var nuevoarraylibres = strlibres.split(",");
        $("#dropbutacao").html("");
        $("#dropbutacao").append($("<option></option>").attr("value", 0).text("Seleccione una butaca"))
        for (i = 0; i < nuevoarraylibres.length; i++)
            $("#dropbutacao").append($("<option></option>").attr("value", nuevoarraylibres[i]).text(nuevoarraylibres[i]));
    }
    $("#btncambiarhorario").click(function () {
        var drophorarios = document.getElementById('drophorarioo');
        var dropasientos = document.getElementById('dropbutacao');
        if (drophorarios.selectedIndex < 1)
            mensajeadvertencia("No ha seleccionado un horario valido");
        else if (dropasientos.selectedIndex < 1)
            mensajeadvertencia("No ha seleccionado una butaca");
        else {
            var arreglodatos = drophorarios.value.split("|");
            var drophorario = document.getElementById('drophorarioo');
            horarioocupado = drophorario.options[drophorario.selectedIndex].text;
            idhorarioocupado = arreglodatos[0];
            idrutaocupado = arreglodatos[1];
            idempresaocupado = arreglodatos[2];
            idservicioocupado = arreglodatos[3];
            butacaocupadocrear = dropasientos.value;
            butacaocupadoanular = document.getElementById('txtbutaca').value;
            fechaviajeocupado = document.getElementById('txtfechaviajeo').value;
            var fecha = document.getElementById("TxtFecha").value;
            var hora = document.getElementById("DropHora").value;
            
            setTimeout('cerrarmodal("facturacion-modal");', 50);
            document.getElementById('lblcontrol').innerHTML = 'cambiarhorario';
            document.getElementById('TxtSubTotalFac').value = recargocambiohorario;
            document.getElementById('TxtTotalFac').value = recargocambiohorario;
        }
    });
    /*****************************************************************************************/
}
$(document).ready(function () {
    $(".chks").live('click', function () {
        var asientos = (this.id).split("-");
        var TxtAsientos = document.getElementById("TxtAsientosR").value;
        var ArrayAsientos = TxtAsientos.split(",");
        var asientosNuevos = "";
        if (document.getElementById(this.id).checked != true) {
            $.each(ArrayAsientos, function (index) {
                if (asientos[1] != ArrayAsientos[index]) {
                    if (asientosNuevos != "") {
                        asientosNuevos = asientosNuevos + "," + ArrayAsientos[index];
                    } else {
                        asientosNuevos = ArrayAsientos[index];
                    }
                }
            });
            document.getElementById("TxtAsientosR").value = asientosNuevos;
        } else {
            if (document.getElementById("TxtAsientosR").value != "") {
                document.getElementById("TxtAsientosR").value = document.getElementById("TxtAsientosR").value + "," + asientos[1];
            } else {
                document.getElementById("TxtAsientosR").value = asientos[1];
            }
        }
    });
})
function boletocomprado(valor) {
    if (valor.substring(0, 1) == "B") {
        valor = valor.substring(6, valor.length);
    }
    var drophora = document.getElementById('DropHora').value;
    var arreglodrophora = drophora.split("|");
    var fecha = formatofecha(document.getElementById('TxtFecha').value);
    var idhorario = arreglodrophora[0];
    var idruta = arreglodrophora[1];
    var idempresa = arreglodrophora[2];
    var idservicio = arreglodrophora[3];
    var idorigen = document.getElementById('DropOrigen').value;
    var iddestino = document.getElementById('DropDestino').value;
    $.ajax({
        type: "POST",
        url: "WSboletotemporal.asmx/datosBoletocomprado",
        data: '{numerobutaca:' + valor + ',idhorario:' + idhorario + ',idorigen:' + idorigen + ',iddestino:' + iddestino + ',idempresa:' + idempresa + ',idservicio:' + idservicio + ',fecha:"' + fecha + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: datosboletocomprado,
        error: errordatosboletocomprado
    });
    function datosboletocomprado(msg) {
        console.log(msg.d);
        var idorigen = 0;
        var iddestino = 0;
        var fecha = 0;
        var idhorario = 0;
        var idservicio = 0;
        $("#ocupados-modal").modal();
        $.each(msg.d, function () {
            document.getElementById('lbltotalo').innerHTML = redondea(this.total);
            document.getElementById('lblrecargocambiohorario').innerHTML = redondea(this.preciorecargo);
            recargocambiohorario = redondea(this.preciorecargo);
            document.getElementById('txtfechaviajeo').value = this.fechaviaje;
            document.getElementById('txtnombreo').value = this.nitcliente;
            document.getElementById('txtviajeo').value = this.origen + "-" + this.destino;
            document.getElementById('txtbutaca').value = this.numerobutaca;
            document.getElementById('lblidservicioo').innerHTML = this.idservicio;
            document.getElementById('lblidboletoanular').innerHTML = this.idboleto;
            idboletoanular = this.idboleto;
            iddestinoocupado = this.iddestino;
            idorigenocupado = this.idorigen;
            idservicioocupado = this.idservicio;
            idhorarioocupado = this.idhorario;
            fechaviajeocupado = this.fechaviaje;
            origenocupado = this.origen;
            destinoocupado = this.destino;
        });
        determinahorarios(idorigenocupado, iddestinoocupado, fechaviajeocupado);
    }
    function errordatosboletocomprado(msg) { mensajeerrorpermanente(msg.responseText); }
    //***********************PROCEDIMIENTO PARA EL DROP DE ORIGEN Y DESTINO************************************//
    function determinahorarios(origen, destino, fecha) {
        fecha = formatofecha(fecha);
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/HoraSugeridaAjax",
            data: '{Origen: "' + origen + '",Destino: "' + destino + '",Fecha: "' + fecha + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: resultadodeterminahorarios,
            async: false,
            error: erroresdeterminahorarios
        });
    }
    /********FUNCION QUE DESPLIEGA LOS HORARIOS RETORNADOS EN EL DROP HORA**********/
    function resultadodeterminahorarios(msg) {
        $("#drophorarioo").html("");
        $.each(msg.d, function () {
            $("#drophorarioo").append($("<option></option>").attr("value", this.datos).text(this.hora));
        });
        var drophorario = document.getElementById('drophorarioo');
        drophorario.selectedIndex = document.getElementById('DropHora').selectedIndex;
    }
    function erroresdeterminahorarios(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
}

$(document).ready(function () {

    $('#BtnSiguiente').click(function () {
        var valor = document.getElementById('TxtAsientos').value;
        var fecha = document.getElementById('TxtFecha').value;
        var nombre = document.getElementById('TxtNombre').value;
        if (!valor) {
            mensajeadvertencia("No ha ingresado asientos");
            document.getElementById('TxtAsientos').focus();
        }
        else if (!fecha) {
            mensajeadvertencia("No ha ingresado fecha de viaje");
            document.getElementById('TxtFecha').focus();
        }
        else if (!nombre) {
            mensajeadvertencia("No ha ingresado nombre del cliente");
        }
        else {
            document.getElementById('DropOrigenVuelta').value = document.getElementById('DropDestino').value;
            document.getElementById('DropDestinoVuelta').value = document.getElementById('DropOrigen').value;
            setTimeout("document.getElementById('DropOrigenVuelta').focus();", 1000);
            document.getElementById('fieldsetida').style.display = 'none';
            document.getElementById('contenedorbus').style.display = 'none';
            document.getElementById('fieldsetretorno').style.display = '';
            document.getElementById('contenedorbusvuelta').style.display = '';
            document.getElementById('TxtFechaVuelta').value = datetomorrow(fecha);
        }
    });
});
$(document).ready(function () {
    $("#DropHora").bind('blur change', function () {
        var valor = document.getElementById("TxtAsientos").value;
        if (valor != "") {
            document.getElementById("TxtAsientos").value = "";
        }
        var Datos= document.getElementById("DropHora").value;
        if(Datos==0){
        return;
        }
        var DatosHora=Datos.split('|');
        var Origen = document.getElementById("DropOrigen").value;
         $.ajax({
            type: "POST",
            url: "WSboleto.asmx/MostrarDestinosRuta",
            data: '{Ruta: "' + DatosHora[1] + '",Origen: "' + Origen + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success:function(data){
                var items = data.d;
                $("#DropDestino").html('');
            $.each(items, function (index, item) {
                    if(item.nombre!=""){
                        if(item.sugerencia=="SI"){
                            $("#DropDestino").append('<option value="' + item.id + '" Selected>' + item.nombre + '</option>');
                            $("#DropDestinoReservado").append('<option value="' + item.id + '" Selected>' + item.nombre + '</option>');
                        
                        }else{
                            $("#DropDestino").append('<option value="' + item.id + '">' + item.nombre + '</option>');
                            $("#DropDestinoReservado").append('<option value="' + item.id + '">' + item.nombre + '</option>');
                        }
                    }
                })
            },
            error: function(data){
                mensajeerrorpermanente("Error: " + data.responseText);
            }
        });
    })
})
$(document).ready(function () {
    $("#DropHoraReasignar").bind('change blur', function () {
        var valor = document.getElementById("TxtAsientoAsignacion").value;
        if (valor != "") {
            document.getElementById("TxtAsientoAsignacion").value = "";
        }
        var Datos = document.getElementById("DropHoraReasignar").value;
        if (Datos == 0) {
            return;
        }
        var DatosHora = Datos.split('|');
        var Origen = document.getElementById("DropOrigenReasignar").value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/MostrarDestinosRuta",
            data: '{Ruta: "' + DatosHora[1] + '",Origen: "' + Origen + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var items = data.d;
                $("#DropDestinoReasignar").html('');
                $.each(items, function (index, item) {
                    if (item.nomnbre != "") {
                        if (item.sugerencia == "SI") {
                            $("#DropDestinoReasignar").append('<option value="' + item.id + '" Selected>' + item.nombre + '</option>');
                        } else {
                            $("#DropDestinoReasignar").append('<option value="' + item.id + '">' + item.nombre + '</option>');
                        }
                    }
                })
            },
            error: function (data) {
                mensajeerrorpermanente("Error: " + data.responseText);
            }
        });
    })
})
$(document).ready(function () {
    $("#DropHoraReservado").bind('change blur', function () {
        var valor = document.getElementById("TxtAsientosReservado").value;
        if (valor != "") {
            document.getElementById("TxtAsientosReservado").value = "";
        }
        var Datos = document.getElementById("DropHoraReservado").value;
        if (Datos == 0) {
            return;
        }
        var DatosHora = Datos.split('|');
        var Origen = document.getElementById("DropOrigenReservado").value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/MostrarDestinosRuta",
            data: '{Ruta: "' + DatosHora[1] + '",Origen: "' + Origen + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var items = data.d;
                $("#DropDestinoReservado").html('');
                $.each(items, function (index, item) {
                    if (item.nomnbre != "") {
                        if (item.sugerencia == "SI") {
                            $("#DropDestinoReservado").append('<option value="' + item.id + '" Selected>' + item.nombre + '</option>');
                        } else {
                            $("#DropDestinoReservado").append('<option value="' + item.id + '">' + item.nombre + '</option>');
                        }
                    }
                })
            },
            error: function (data) {
                mensajeerrorpermanente("Error: " + data.responseText);
            }
        });
    })
})
$(document).ready(function () {
    $("#DropHoraVuelta").bind('click blur change', function () {
        var valor = document.getElementById("TxtAsientosVuelta").value;
        if (valor != "") {
            document.getElementById("TxtAsientosVuelta").value = "";
        }
        var Datos = document.getElementById("DropHoraVuelta").value;
        if (Datos == 0) {
            return;
        }
        var DatosHora = Datos.split('|');
        var Origen = document.getElementById("DropOrigenVuelta").value;
        $.ajax({
            type: "POST",
            url: "WSboleto.asmx/MostrarDestinosRuta",
            data: '{Ruta: "' + DatosHora[1] + '",Origen: "' + Origen + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var items = data.d;
                $("#DropDestinoVuelta").html('');
                $.each(items, function (index, item) {
                    if (item.nomnbre != "") {
                        if (item.sugerencia == "SI") {
                            $("#DropDestinoVuelta").append('<option value="' + item.id + '" Selected>' + item.nombre + '</option>');
                        } else {
                            $("#DropDestinoVuelta").append('<option value="' + item.id + '">' + item.nombre + '</option>');
                        }
                    }
                })
            },
            error: function (data) {
                mensajeerrorpermanente("Error: " + data.responseText);
            }
        });
    })
})

/*NUEVA FUNCION*/
function muestrabutacasocupadasVuelta(fecha,hora,ruta,empresa,origen,destino,servicio) {

    $.ajax({
        type: "POST",
        url: "WSboleto.asmx/DeterminaAsientos",
        data: '{Fecha: "' + fecha + '",Hora: "' + hora + '",Ruta: "' + ruta + '",Empresa: "' + empresa + '",Origen: "' + origen + '",Destino: "' + destino + '",Servicio: "' + servicio + '"}',
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
        success: resultadoasientosvuelta,
        errorasientos: erroresasientosvuelta
    });
    function erroresasientosvuelta(msg) {
        mensajeerrorpermanente('Error: ' + msg.responseText);
    }
    function resultadoasientosvuelta(msg) {
        var asientos = msg.d;
        var ocupados = 0;
        var reservados = 0;
        $.each(asientos, function (index, asiento) {
            var nuevoasiento = document.getElementById("Button" + asiento.asientonumero);
            nuevoasiento.className = asiento.asientoestado;
            nuevoasiento.title = asiento.cliente + "|DESTINO: " + asiento.viaje + "|TOTAL: " + asiento.total;
            if (asiento.asientoestado == "ocupado") {
                nuevoasiento.className = "ocupado";
                ocupados++;
            }
            else {
                nuevoasiento.className = "reservado";
                reservados++;
            }
        });
        /*MOSTRAMOS EL NUMERO DE ASIENTOS OCUPADOS,RESERVADOS Y LIBRES*/
        document.getElementById('thocupado').innerHTML = ocupados;
        document.getElementById('threservado').innerHTML = reservados;
        var contadorlibres = 0;
        $("#contenedorbusvuelta .libre").each(function (index) {
            contadorlibres++;
        })
        document.getElementById('thlibre').innerHTML = contadorlibres;
    }

}
/*NUEVA FUNCION*/