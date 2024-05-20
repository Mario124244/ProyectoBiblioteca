$(document).ready(function () {
    var selectedSeat = null;
    var reservas = []; // Array para guardar las reservas

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function cargarCubiculos() {
        $.get("https://localhost:7230/api/reservations/cubiculos", function (response) {
            var cubiculos = response;

            // Verificar si la respuesta contiene la propiedad $values
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
        var userId = 1; // ID del usuario, reemplazar con el valor correcto
        var cubiculoId = $(selectedSeat).data("id"); // ID del cubículo seleccionado

        console.log("horaEntrada:", horaEntrada);
        console.log("horaSalida:", horaSalida);
        console.log("cubiculoId:", cubiculoId);

        if (horaEntrada && horaSalida && cubiculoId) {
            var entrada = new Date("1970-01-01T" + horaEntrada + "Z");
            var salida = new Date("1970-01-01T" + horaSalida + "Z");
            var diferencia = (salida - entrada) / (1000 * 60 * 60); // Diferencia en horas

            console.log("diferencia:", diferencia);

            if (diferencia <= 0 || diferencia > 3) {
                $('#errorMessage').text('La reserva no puede ser mayor a 3 horas o menor que 0 horas.').show();
            } else {
                try {
                    const response = await fetch('https://localhost:7230/api/reservations', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': getAntiForgeryToken() // Incluir el token antifalsificación
                        },
                        body: JSON.stringify({
                            userId: userId,
                            cubiculoId: cubiculoId,
                            startDate: entrada.toISOString(),
                            endDate: salida.toISOString()
                        })
                    });

                    const result = await response.text();
                    console.log("result:", result);

                    if (!response.ok) {
                        $('#errorMessage').text(result).show();
                    } else {
                        reservas.push({ entrada: horaEntrada, salida: horaSalida });
                        $(selectedSeat).removeClass('available').addClass('occupied');
                        $('#reservationModal').modal('hide');
                        alert('Reserva confirmada: ' + horaEntrada + ' a ' + horaSalida);
                    }
                } catch (error) {
                    $('#errorMessage').text(`Ocurrió un error al intentar realizar la reserva: ${error.message}`).show();
                }
            }
        } else {
            $('#errorMessage').text('Por favor, complete todos los campos.').show();
        }
    });

    // Función para ocultar el mensaje de error cuando se cierra el modal
    $('#reservationModal').on('hidden.bs.modal', function () {
        $('#errorMessage').hide().text('');
    });
});

