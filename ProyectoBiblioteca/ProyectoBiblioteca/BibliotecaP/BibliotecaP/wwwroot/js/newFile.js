$(document).ready(function() {
    var selectedSeat = null;
    var ultimaReserva = null;
    var userId = 1; // Obtener el userId desde un campo oculto en el HTML, si es necesario


    // Conectar al hub de SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/cubiculoHub")
        .build();

    connection.on("CubiculoEstadoActualizado", function(cubiculoId, estado) {
        var seat = $(`[data-id='${cubiculoId}']`);
        seat.removeClass('available occupied maintenance');
        switch (estado) {
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
    });

    connection.on("MesaEstadoActualizado", function(mesaId, estado) {
        var table = $(`[data-id='${mesaId}']`);
        table.removeClass('available occupied maintenance');
        switch (estado) {
            case 'Disponible':
                table.addClass('available');
                break;
            case 'Ocupado':
                table.addClass('occupied');
                break;
            case 'Mantenimiento':
                table.addClass('maintenance');
                break;
        }
    });

    connection.start().catch(function(err) {
        return console.error(err.toString());
    });

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function cargarCubiculos() {
        $.get("https://localhost:7230/api/reservations/cubiculos", function(response) {
            var cubiculos = response.$values || response;

            cubiculos.forEach(function(cubiculo) {
                var seat = $(`[data-id='${cubiculo.cubiculoId}']`);
                seat.removeClass('available occupied maintenance');
                if (cubiculo.estadoNombre != null) {
                    switch (cubiculo.estadoNombre) {
                        case 'Disponible':
                            seat.addClass('available');
                            break;
                        case 'Ocupado':
                            seat.addClass('unavailable');
                            break;
                        case 'Mantenimiento':
                            seat.addClass('maintenance');
                            break;
                    }
                }
            });

            $('.seat.available').on('click', function() {
                if (selectedSeat) {
                    $(selectedSeat).removeClass('selected');
                }
                selectedSeat = this;
                $(this).addClass('selected');
            });
        }).fail(function() {
            alert('Error al cargar los cubículos');
        });
    }

    function cargarMesas() {
        $.get("https://localhost:7230/api/reservations/mesas", function(response) {
            var mesas = response.$values || response;

            mesas.forEach(function(mesa) {
                var table = $(`[data-id='${mesa.mesaId}']`);
                table.removeClass('available occupied maintenance');
                if (mesa.estadoNombre != null) {
                    switch (mesa.estadoNombre) {
                        case 'Disponible':
                            table.addClass('available');
                            break;
                        case 'Ocupado':
                            table.addClass('unavailable');
                            break;
                        case 'Mantenimiento':
                            table.addClass('maintenance');
                            break;
                    }
                }
            });

            $('.table.available').on('click', function() {
                if (selectedSeat) {
                    $(selectedSeat).removeClass('selected');
                }
                selectedSeat = this;
                $(this).addClass('selected');
            });
        }).fail(function() {
            alert('Error al cargar las mesas');
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
    cargarMesas();

    $('.reservar-cubiculo').on('click', function() {
        if (selectedSeat) {
            $('#reservationModal').modal('show');
        } else {
            alert('Por favor, seleccione un cubículo primero.');
        }
    });

    $('.reservar-mesa').on('click', function() {
        if (selectedSeat) {
            $('#reservationMesaModal').modal('show');
        } else {
            alert('Por favor, seleccione una mesa primero.');
        }
    });

    $('#confirmarReservaCubiculo').on('click', async function() {
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
                    const response = await fetch('https://localhost:7230/api/reservations/cubiculo', {
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

                        // Notificar a todos los clientes sobre el cambio de estado
                        await connection.invoke("CubiculoEstadoActualizado", cubiculoId, "Ocupado").catch(function(err) {
                            return console.error(err.toString());
                        });

                        // Actualizar el estado de los cubículos sin recargar la página
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

    $('#confirmarReservaMesa').on('click', async function() {
        var horaEntrada = $('#horaEntradaMesa').val();
        var horaSalida = $('#horaSalidaMesa').val();
        var mesaId = $(selectedSeat).data("id");

        await obtenerUltimaReserva();

        if (horaEntrada && horaSalida && mesaId) {
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
                    const response = await fetch('https://localhost:7230/api/reservations/mesa', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': getAntiForgeryToken()
                        },
                        body: JSON.stringify({
                            userId: userId,
                            mesaId: mesaId,
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
                        $('#reservationMesaModal').modal('hide');

                        // Notificar a todos los clientes sobre el cambio de estado
                        await connection.invoke("MesaEstadoActualizado", mesaId, "Ocupado").catch(function(err) {
                            return console.error(err.toString());
                        });

                        // Actualizar el estado de las mesas sin recargar la página
                        cargarMesas();

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

    $('#reservationModal').on('hidden.bs.modal', function() {
        $('#errorMessage').hide().text('');
    });

    $('#reservationMesaModal').on('hidden.bs.modal', function() {
        $('#errorMessageMesa').hide().text('');
    });
});
