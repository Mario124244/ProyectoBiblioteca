$(document).ready(function () {
    var selectedSeat = null;
    var ultimaReserva = null;
    var userId = 1; // Obtener el userId desde un campo oculto en el HTML, si es necesario

    // Conectar al hub de SignalR
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/cubiculoHub")
        .build();

    connection.on("CubiculoEstadoActualizado", function (cubiculoId, estado) {
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

    connection.on("MesaEstadoActualizado", function (mesaId, estado) {
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

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }

    function cargarCubiculos() {
        $.get("https://localhost:7230/api/reservations/cubiculos", function (response) {
            var cubiculos = response.$values || response;

            cubiculos.forEach(function (cubiculo) {
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

    function cargarMesas() {
        $.get("https://localhost:7230/api/reservations/mesas", function (response) {
            var mesas = response.$values || response;

            mesas.forEach(function (mesa) {
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

            $('.table.available').on('click', function () {
                if (selectedSeat) {
                    $(selectedSeat).removeClass('selected');
                }
                selectedSeat = this;
                $(this).addClass('selected');
            });
        }).fail(function () {
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
                console.log('Última reserva obtenida:', ultimaReserva); // Verificar el valor de ultimaReserva
            }
        } catch (error) {
            console.error('Error al obtener el historial de reservas:', error);
        }
    }

    cargarCubiculos();
    cargarMesas();
