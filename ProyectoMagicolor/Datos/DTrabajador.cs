﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Datos
{
    public class DTrabajador : Conexion
    {
        private int _idTrabajador;
        public int idTrabajador
        {
            get { return _idTrabajador; }
            set { _idTrabajador = value; }
        }

        private string _Nombre;
        public string nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Apellidos;
        public string apellidos
        {
            get { return _Apellidos; }
            set { _Apellidos = value; }
        }

        private string _Sexo;
        public string sexo
        {
            get { return _Sexo; }
            set { _Sexo = value; }
        }

        private DateTime _FechaNacimiento;
        public DateTime fechaNacimiento
        {
            get { return _FechaNacimiento; }
            set { _FechaNacimiento = value; }
        }

        private string _Cedula;
        public string cedula
        {
            get { return _Cedula; }
            set { _Cedula = value; }
        }

        private string _Direccion;
        public string direccion
        {
            get { return _Direccion; }
            set { _Direccion = value; }
        }

        private string _Telefono;
        public string telefono
        {
            get { return _Telefono; }
            set { _Telefono = value; }
        }

        private string _Email;
        public string email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private int _Acceso;
        public int acceso
        {
            get { return _Acceso; }
            set { _Acceso = value; }
        }

        private string _Usuario;
        public string usuario
        {
            get { return _Usuario; }
            set { _Usuario = value; }
        }

        private string _Contraseña;
        public string contraseña
        {
            get { return _Contraseña; }
            set { _Contraseña = value; }
        }

        private int _Estado;
        public int estado
        {
            get { return _Estado; }
            set { _Estado = value; }
        }

        private string _Pregunta;
        public string pregunta
        {
            get { return _Pregunta; }
            set { _Pregunta = value; }
        }

        private string _Respuesta;
        public string respuesta
        {
            get { return _Respuesta; }
            set { _Respuesta = value; }
        }

        private int _AccesoTrabajadorIngresado;
        public int accesoTrabajadorIngresado
        {
            get { return _AccesoTrabajadorIngresado; }
            set { _AccesoTrabajadorIngresado = value; }
        }

        private int _TrabajadorIngresado;
        public int trabajadorIngresado
        {
            get { return _TrabajadorIngresado; }
            set { _TrabajadorIngresado = value; }
        }

        private string _AccesoString;
        public string accesoString
        {
            get { return _AccesoString; }
            set { _AccesoString = value; }
        }

        private string _NombreTrabajadorIngresado;
        public string nombreTrabajadorIngresado
        {
            get { return _NombreTrabajadorIngresado; }
            set { _NombreTrabajadorIngresado = value; }
        }

        public DTrabajador()
        {

        }

        public DTrabajador(int IdTrabajador, string Nombre, string Apellidos, string Sexo, DateTime FechaNacimiento, string Cedula, string Direccion,string Telefono, string Email, int Acceso, string Usuario, string Contraseña)
        {
            this.idTrabajador = IdTrabajador;
            this.nombre = Nombre;
            this.apellidos = Apellidos;
            this.sexo = Sexo;
            this.fechaNacimiento = FechaNacimiento;
            this.cedula = Cedula;
            this.direccion = Direccion;
            this.telefono = Telefono;
            this.email = Email;
            this.acceso = Acceso;
            this.usuario = Usuario;
            this.contraseña = Contraseña;
        }
    }
}
