$(document).ready(function () {
    var selectedSeat = null;
    var ultimaReserva = null;
    var userId = 1; // Obtener el userId desde un campo oculto en el HTML, si es necesario

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function cargarCubiculos() {
        $.get("https://localhost:7230/api/reservations/cubiculos", function (response) {
            var cubiculos = response.$values || response;

            cubiculos.forEach(function (cubiculo) {
                var seat = $(`[data-id='${cubiculo.cubiculoId}']`);
                seat.removeClass('available occupied maintenance');
                if (cubiculo.estado && cubiculo.estado.nombre) {
                    switch (cubiculo.estado.nombre) {
                        case 'Disponible':
                            seat.addClass('available');
                            break;
                        case 'Ocupado':
                            seat.addClass('occupied');
                            break;
                        case 'Mantenimiento':
                            seat.addClass('maintenance');
                            break;
                    }
                }
            });

            $('.seat.available').on('click', function () {
                if (selectedSeat) {
                    $(selectedSeat).removeClass('selected');
                }
                selectedSeat = this;
                $(this).addClass('selected');
            });
        }).fail(function () {
            alert('Error al cargar los cubículos');
        });
    }

    async function obtenerUltimaReserva() {
        try {
            const response = await fetch(`https://localhost:7230/api/reservations/user/${userId}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const reservas = await response.json();
            if (reservas && reservas.length > 0) {
                ultimaReserva = new Date(reservas[0].fechaHoraInicio);
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
        var cubiculoId = $(selectedSeat).data("id");

        await obtenerUltimaReserva();

        if (horaEntrada && horaSalida && cubiculoId) {
            var entrada = new Date();
            var salida = new Date();

            var [entradaHoras, entradaMinutos] = horaEntrada.split(":");
            entrada.setHours(entradaHoras, entradaMinutos, 0, 0);
            entrada.setMilliseconds(0);

            var [salidaHoras, salidaMinutos] = horaSalida.split(":");
            salida.setHours(salidaHoras, salidaMinutos, 0, 0);
            salida.setMilliseconds(0);

            var diferencia = (salida - entrada) / (1000 * 60 * 60);
            var ahora = new Date();

            if (ultimaReserva && ((ahora - ultimaReserva) < 3 * 60 * 60 * 1000)) {
                $('#errorMessage').text('Debe esperar al menos 3 horas entre reservas.').show();
                return;
            }

            if (diferencia <= 0 || diferencia > 3) {
                $('#errorMessage').text('La reserva no puede ser mayor a 3 horas o menor que 0 horas.').show();
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

                    if (!response.ok) {
                        const result = await response.text();
                        $('#errorMessage').text(result).show();
                    } else {
                        const result = await response.json();
                        ultimaReserva = new Date();
                        $(selectedSeat).removeClass('available').addClass('occupied');
                        $('#reservationModal').modal('hide');

                        cargarCubiculos();

                        const reservaId = result.reservacionId;
                        window.location.href = `/Usuario/ReservaInfo?reservaId=${reservaId}`;
                    }
                } catch (error) {
                    $('#errorMessage').text(`Ocurrió un error al intentar realizar la reserva: ${error.message}`).show();
                }
            }
        } else {
            $('#errorMessage').text('Por favor, complete todos los campos.').show();
        }
    });

    $('#reservationModal').on('hidden.bs.modal', function () {
        $('#errorMessage').hide().text('');
    });
});
