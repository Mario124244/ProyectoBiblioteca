$(document).ready(function () {
    var selectedSeat = null;
    var ultimaReserva = null; // Variable para almacenar la hora de la última reserva
    var userId = 1; // ID del usuario, reemplazar con el valor correcto

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function cargarCubiculos() {
        $.get("https://localhost:7230/api/reservations/cubiculos", function (response) {
            var cubiculos = response;

            if (response.hasOwnProperty('$values')) {
                cubiculos = response.$values;
            }

            if (Array.isArray(cubiculos)) {
                cubiculos.forEach(function (cubiculo) {
                    if (cubiculo && cubiculo.cubiculoId && cubiculo.estado && cubiculo.estado.nombre) {
                        var seat = $(`[data-id='${cubiculo.cubiculoId}']`);
                        if (cubiculo.estado.nombre === 'Disponible') {
                            seat.addClass('available').removeClass('occupied maintenance');
                        } else if (cubiculo.estado.nombre === 'Ocupado') {
                            seat.addClass('occupied').removeClass('available maintenance');
                        } else if (cubiculo.estado.nombre === 'Mantenimiento') {
                            seat.addClass('maintenance').removeClass('available occupied');
                        }
                    } else {
                        console.warn(`Cubículo con ID ${cubiculo ? cubiculo.cubiculoId : 'undefined'} no tiene un estado válido.`);
                    }
                });

                $('.seat.available').on('click', function () {
                    if (selectedSeat) {
                        $(selectedSeat).removeClass('selected');
                    }
                    selectedSeat = this;
                    $(this).addClass('selected');
                });
            } else {
                console.error('La respuesta de la API no es un array:', response);
                alert('Error al cargar los cubículos');
            }
        }).fail(function () {
            alert('Error al cargar los cubículos');
        });
    }

    async function obtenerUltimaReserva() {
        try {
            const response = await fetch(`https://localhost:7230/api/reservations/user/${userId}`);
            const reservas = await response.json();
            if (reservas && reservas.length > 0) {
                ultimaReserva = new Date(reservas[0].fechaHoraInicio); // Asegúrate de que esta propiedad sea correcta
                console.log("Ultima reserva obtenida:", ultimaReserva);
            }
        } catch (error) {
            console.error('Error al obtener el historial de reservas:', error);
        }
    }

    cargarCubiculos();

    $('.reservar').on('click', function () {
        if (selectedSeat) {
            $('#reservationModal').modal('show');
        } else {
            alert('Por favor, seleccione una butaca primero.');
        }
    });

    $('#confirmarReserva').on('click', async function () {
        var horaEntrada = $('#horaEntrada').val();
        var horaSalida = $('#horaSalida').val();
        var cubiculoId = $(selectedSeat).data("id"); // ID del cubículo seleccionado

        await obtenerUltimaReserva(); // Obtener la última reserva antes de confirmar

        if (horaEntrada && horaSalida && cubiculoId) {
            var entrada = new Date("1970-01-01T" + horaEntrada + "Z");
            var salida = new Date("1970-01-01T" + horaSalida + "Z");
            var diferencia = (salida - entrada) / (1000 * 60 * 60); // Diferencia en horas
            var ahora = new Date(); // Hora actual

            console.log("horaEntrada:", horaEntrada);
            console.log("horaSalida:", horaSalida);
            console.log("cubiculoId:", cubiculoId);
            console.log("ultimaReserva antes de la reserva:", ultimaReserva);
            console.log("ahora:", ahora);

            // Verificar si han pasado al menos 3 horas desde la última reserva
            if (ultimaReserva && ((ahora - ultimaReserva) < 3 * 60 * 60 * 1000)) {
                $('#errorMessage').text('Debe esperar al menos 3 horas entre reservas.').show();
                console.log("Debe esperar al menos 3 horas entre reservas.");
                return;
            }

            if (diferencia <= 0 || diferencia > 3) {
                $('#errorMessage').text('La reserva no puede ser mayor a 3 horas o menor que 0 horas.').show();
                console.log("La reserva no puede ser mayor a 3 horas o menor que 0 horas.");
            } else {
                try {
                    const response = await fetch('https://localhost:7230/api/reservations', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': getAntiForgeryToken()
                        },
                        body: JSON.stringify({
                            userId: userId,
                            cubiculoId: cubiculoId,
                            startDate: entrada.toISOString(),
                            endDate: salida.toISOString()
                        })
                    });

                    const result = await response.text();

                    if (!response.ok) {
                        $('#errorMessage').text(result).show();
                    } else {
                        ultimaReserva = new Date(); // Actualizar la hora de la última reserva
                        console.log("Nueva ultimaReserva después de la reserva:", ultimaReserva);
                        $(selectedSeat).removeClass('available').addClass('occupied');
                        $('#reservationModal').modal('hide');
                        alert('Reserva confirmada: ' + horaEntrada + ' a ' + horaSalida);
                    }
                } catch (error) {
                    $('#errorMessage').text(`Ocurrió un error al intentar realizar la reserva: ${error.message}`).show();
                    console.log("Error al intentar realizar la reserva:", error.message);
                }
            }
        } else {
            $('#errorMessage').text('Por favor, complete todos los campos.').show();
            console.log("Por favor, complete todos los campos.");
        }
    });

    $('#reservationModal').on('hidden.bs.modal', function () {
        $('#errorMessage').hide().text('');
    });
});
