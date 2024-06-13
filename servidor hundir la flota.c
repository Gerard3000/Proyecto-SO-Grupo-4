#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>
#include <ctype.h>
#include <mysql.h>

int server = 1; // 1 = servidor shiva, 0 = localhost
int pSvr = 50075; //Puerto que utilizaremos en el servidor
int pLcl = 6060; //Puerto que utilizaremos en local

typedef struct{
	char usuario[20];
	int socket;
	int partida;
} Tusuario;

typedef struct{
	Tusuario usuarios[512];
	int num;
} Tconectados;
Tconectados conectados;
/*
typedef struct{
	int invita;
	int invitado;
} Tinvitacion;

typedef struct{
	Tinvitacion inv[512];
	int num;
} Tinvitaciones;
Tinvitaciones invitaciones;
*/


typedef struct{
	int bote[3];
	int bergantin1[3];
	int bergantin2[3];
	int posBergantin;
	int destructor[4];
	int portaaviones[5];
	int barcoshundidos;
}Ttablero;

typedef struct{
	Ttablero tablero[4]; //0 sera el que cree la invitacion, 1 el invitado, 2 copia del 0, 3 copia del 1
	int socket[2]; //0 sera el que cree la invitacion, 1 el invitado. Lo utilizaremos para identificar quien es quien
	int estadoP; //Estado de la partida
	int estadoT[2]; //0 sera el que cree la invitacion, 1 el invitado
	int puntuacion[2]; //0 sera el que cree la invitacion, 1 el invitado. UNDIR BOTE = 10 PUNTOS, UNDIR BERGANTIN 30 PUNTOS, UNDIR DESTRUCTOR 40 PUNTOS, UNDIR PORTAAVIONES 50 PUNTOS;
}Tpartida;

typedef struct{
	Tpartida partidas[512];
	int num;
}Tjuego;
Tjuego part;

int msgs = 0;

int contador;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int i;
int sockets[100];

void broadcast(char enviar[200]){
	for(int w=0; w<conectados.num; w++)
	write(conectados.usuarios[w].socket, enviar, strlen(enviar));
}

int EliminarConectado(char usuario[20]){
	char enviar[200];
	char lista[200];
	for(int j=0; j<conectados.num; j++){
		if(strcmp(conectados.usuarios[j].usuario, usuario)==0){
			pthread_mutex_lock( &mutex );
			for(int w=j; w<conectados.num-1; w++){
				if(w!=conectados.num-1){
					strcpy(conectados.usuarios[w].usuario, conectados.usuarios[w+1].usuario);
					conectados.usuarios[w].socket = conectados.usuarios[w+1].socket;
				}
			}
			conectados.num--;
			pthread_mutex_unlock( &mutex);
			sprintf(lista, "%s", conectados.usuarios[0].usuario);
			if(conectados.num>1){
				for(int w=1; w<conectados.num; w++)
					sprintf(lista, "%s,%s" ,lista, conectados.usuarios[w].usuario);
			}
			printf("Lista= %s\n", lista);
			sprintf(enviar,"4/%s",lista);
			printf("%s\n" ,enviar);
			broadcast(enviar);
			return 1;
		}
	}
	return -1;
}

void AnadirConectado(char usuario[20]){
	char enviar[200];
	char lista[200];
	pthread_mutex_lock( &mutex );
	strcpy(conectados.usuarios[conectados.num].usuario, usuario);
	conectados.usuarios[conectados.num].socket = sockets[i];
	conectados.usuarios[conectados.num].partida = -1;
	conectados.num++;
	pthread_mutex_unlock( &mutex);
	printf("Nuevo usuario conectado %d: %s\n" , conectados.num, conectados.usuarios[conectados.num-1].usuario);
	
	sprintf(lista, "%s", conectados.usuarios[0].usuario);
	if(conectados.num>1){
		for(int w=1; w<conectados.num; w++)
			sprintf(lista, "%s,%s" ,lista, conectados.usuarios[w].usuario);
	}
	printf("Lista= %s\n", lista);
	sprintf(enviar,"6/%s",lista);
	printf("%s\n" ,enviar);
	broadcast(enviar);
}

int posicionJugador(int socket){ //Me devuelve la posicion del jugador en la lista de conectados
	for(int j = 0; j<conectados.num; j++){
		if(conectados.usuarios[j].socket == socket){
			return j;
		}
	}
	return -1;
}

void crearPartida(int sock, int sock2){
	int pos1, pos2;
	for(int j=0; j<conectados.num;j++){
		if(conectados.usuarios[j].socket == sock)
			pos1 = j;
		if(conectados.usuarios[j].socket == sock2)
			pos2 = j;
	}
	pthread_mutex_lock(&mutex);
		conectados.usuarios[pos1].partida = 1;
		conectados.usuarios[pos2].partida = 1;
		part.partidas[part.num].estadoP = 0;
		part.partidas[part.num].estadoT[0] = 0;
		part.partidas[part.num].estadoT[1] = 0;
		part.partidas[part.num].puntuacion[0] = 0;
		part.partidas[part.num].puntuacion[1] = 0;
		part.partidas[part.num].socket[0] = sock;
		part.partidas[part.num].socket[1] = sock2;
		part.partidas[part.num].tablero[0].posBergantin = 0;
		part.partidas[part.num].tablero[1].posBergantin = 0;
		part.partidas[part.num].tablero[0].barcoshundidos = 0;
		part.partidas[part.num].tablero[1].barcoshundidos = 0;
		part.num++;
	pthread_mutex_unlock(&mutex);
}

int posicionPartida(int sock){ //Me devuelve la posicion de la partida en el vector de posiciones
	for(int j = 0; j < part.num; j++)
		if(part.partidas[j].socket[0] == sock || part.partidas[j].socket[1] == sock)
			return j;
	return -1;
}

int atacando(int posPart, int enemigo, int pos, int sock_conn, int miPos){
	int status = 0;
	for(int j =0; j<5; j++){ //queremos que busque del 0 al 5 en cada tipo de barco si la posicion coincide, y a parte miramos si hundimos el barco y en caso de hundir barco si la partida acaba por haber undido todos los barcos.
		char msg[200];
		if(j<3){
			if(part.partidas[posPart].tablero[enemigo+2].bote[j] == pos){ //buscar si es un bote undido
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].bote[j] = 1;
				pthread_mutex_unlock(&mutex);
				
				sprintf(msg, "24/%d" ,pos); //poner en rojo en la matriz 1
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				sprintf(msg, "23/%d" ,pos); //poner en rojo en la matriz 2
				write(sock_conn, msg, strlen(msg));
				
				printf("\nPoner bote en negro %d\n" ,pos);
				sprintf(msg, "19/1/%d" ,pos);
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				printf("\nPoner bote en negro %d\n" ,pos);
				sprintf(msg, "20/1/%d" ,pos);
				write(sock_conn, msg, strlen(msg));
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].barcoshundidos++;  //Sumamos 1 a la lista de barcos (7 en total) para poder saber cuantos llevamos hundidos.
					part.partidas[posPart].puntuacion[miPos] += 10;
				pthread_mutex_unlock(&mutex);
				printf("\nNº de barcos hundidos es: %d\n" ,part.partidas[posPart].tablero[enemigo].barcoshundidos);
				if(part.partidas[posPart].tablero[enemigo].barcoshundidos == 7){
					return 2;
				}
				printf("\nDevuelvo 1\n");
				return 1;
			}
			if(part.partidas[posPart].tablero[enemigo].bergantin1[j] == pos){
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].bergantin1[j] = 1;
				pthread_mutex_unlock(&mutex);
				sprintf(msg, "24/%d" ,pos);
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				sprintf(msg, "23/%d" ,pos);
				write(sock_conn, msg, strlen(msg));
				
				int bergUnd = 0;
				bergUnd = bergantinUndido1(posPart, enemigo);
				if(bergUnd == 1){
					int pos1 = part.partidas[posPart].tablero[enemigo+2].bergantin1[0];
					int pos2 = part.partidas[posPart].tablero[enemigo+2].bergantin1[1];
					int pos3 = part.partidas[posPart].tablero[enemigo+2].bergantin1[2];
					sprintf(msg, "19/2/%d,%d,%d" ,pos1, pos2, pos3);
					write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
					sprintf(msg, "20/2/%d,%d,%d" ,pos1, pos2, pos3);
					write(sock_conn, msg, strlen(msg));
					pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].barcoshundidos++; //Sumamos 1 a la lista de barcos (7 en total) para poder saber cuantos llevamos hundidos.
					part.partidas[posPart].puntuacion[miPos] += 30;
					pthread_mutex_unlock(&mutex);
					printf("\nNº de barcos hundidos es: %d\n" ,part.partidas[posPart].tablero[enemigo].barcoshundidos);
					if(part.partidas[posPart].tablero[enemigo].barcoshundidos == 7)
						return 2;
				}
				printf("\nDevuelvo 1\n");
				return 1;
			}
			if(part.partidas[posPart].tablero[enemigo].bergantin2[j] == pos){
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].bergantin2[j] = 1;
				pthread_mutex_unlock(&mutex);
				sprintf(msg, "24/%d" ,pos);
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				sprintf(msg, "23/%d" ,pos);
				write(sock_conn, msg, strlen(msg));
				int bergUnd = 0;
				bergUnd = bergantinUndido2(posPart, enemigo);
				if(bergUnd == 1){
					int pos1 = part.partidas[posPart].tablero[enemigo+2].bergantin2[0];
					int pos2 = part.partidas[posPart].tablero[enemigo+2].bergantin2[1];
					int pos3 = part.partidas[posPart].tablero[enemigo+2].bergantin2[2];
					sprintf(msg, "19/2/%d,%d,%d" ,pos1, pos2, pos3);
					write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
					sprintf(msg, "20/2/%d,%d,%d" ,pos1, pos2, pos3);
					write(sock_conn, msg, strlen(msg));
					pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].barcoshundidos++; //Sumamos 1 a la lista de barcos (7 en total) para poder saber cuantos llevamos hundidos.
					part.partidas[posPart].puntuacion[miPos] += 30;
					pthread_mutex_unlock(&mutex);
					printf("\nNº de barcos hundidos es: %d\n" ,part.partidas[posPart].tablero[enemigo].barcoshundidos);
					status = partidaFinalizada(posPart, enemigo); //partidaFinaliza comprueba el numero de barcos hundidos y devuelve 1 si se han hundido todos.
					if(part.partidas[posPart].tablero[enemigo].barcoshundidos == 7)
						return 2;
				}
				printf("\nDevuelvo 1\n");
				return 1;
			}

		}
		if(j<4){
			if(part.partidas[posPart].tablero[enemigo].destructor[j] == pos){
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].destructor[j] = 1;
				pthread_mutex_unlock(&mutex);
				sprintf(msg, "24/%d" ,pos);
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				sprintf(msg, "23/%d" ,pos);
				write(sock_conn, msg, strlen(msg));
				int destUnd = 0;
				destUnd = destructorUndido(posPart, enemigo);
				if(destUnd == 1){
					int pos1 = part.partidas[posPart].tablero[enemigo+2].destructor[0];
					int pos2 = part.partidas[posPart].tablero[enemigo+2].destructor[1];
					int pos3 = part.partidas[posPart].tablero[enemigo+2].destructor[2];
					int pos4 = part.partidas[posPart].tablero[enemigo+2].destructor[3];
					sprintf(msg, "19/3/%d,%d,%d,%d" ,pos1, pos2, pos3, pos4);
					write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
					sprintf(msg, "20/3/%d,%d,%d,%d" ,pos1, pos2, pos3, pos4);
					write(sock_conn, msg, strlen(msg));
					pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].barcoshundidos++; //Sumamos 1 a la lista de barcos (7 en total) para poder saber cuantos llevamos hundidos.
					part.partidas[posPart].puntuacion[miPos] += 40;
					pthread_mutex_unlock(&mutex);
					printf("\nNº de barcos hundidos es: %d\n" ,part.partidas[posPart].tablero[enemigo].barcoshundidos);
					status = partidaFinalizada(posPart, enemigo); //partidaFinaliza comprueba el numero de barcos hundidos y devuelve 1 si se han hundido todos.
					if(part.partidas[posPart].tablero[enemigo].barcoshundidos == 7)
						return 2;
				}
				printf("\nDevuelvo 1\n");
				return 1;
			}
		}
		if(j<5){
			if(part.partidas[posPart].tablero[enemigo].portaaviones[j] == pos){
				pthread_mutex_lock(&mutex);
					part.partidas[posPart].tablero[enemigo].portaaviones[j] = 1;
				pthread_mutex_unlock(&mutex);
				sprintf(msg, "24/%d" ,pos);
				write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
				sprintf(msg, "23/%d" ,pos);
				write(sock_conn, msg, strlen(msg));
				int portaUnd = 0;
				portaUnd = portaUndido(posPart, enemigo);
				if(portaUnd == 1){
					int pos1 = part.partidas[posPart].tablero[enemigo+2].portaaviones[0];
					int pos2 = part.partidas[posPart].tablero[enemigo+2].portaaviones[1];
					int pos3 = part.partidas[posPart].tablero[enemigo+2].portaaviones[2];
					int pos4 = part.partidas[posPart].tablero[enemigo+2].portaaviones[3];
					int pos5 = part.partidas[posPart].tablero[enemigo+2].portaaviones[4];
					sprintf(msg, "19/4/%d,%d,%d,%d,%d" ,pos1, pos2, pos3, pos4, pos5);
					write(part.partidas[posPart].socket[enemigo], msg, strlen(msg));
					sprintf(msg, "20/4/%d,%d,%d,%d,%d" ,pos1, pos2, pos3, pos4, pos5);
					write(sock_conn, msg, strlen(msg));
					pthread_mutex_lock(&mutex);
						part.partidas[posPart].tablero[enemigo].barcoshundidos++; //Sumamos 1 a la lista de barcos (7 en total) para poder saber cuantos llevamos hundidos.
						part.partidas[posPart].puntuacion[miPos] +=50;
					pthread_mutex_unlock(&mutex);
					printf("\nNº de barcos hundidos es: %d\n" ,part.partidas[posPart].tablero[enemigo].barcoshundidos);
					status = partidaFinalizada(posPart, enemigo); //partidaFinaliza comprueba el numero de barcos hundidos y devuelve 1 si se han hundido todos.
					if(part.partidas[posPart].tablero[enemigo].barcoshundidos == 7)
						return 2;
				}
				printf("\nDevuelvo 1\n");
				return 1;
			}
		}
	}
	return 0;
}
int partidaFinalizada(int partida, int enemigo){
	if (part.partidas[partida].tablero[enemigo].barcoshundidos == 7)
		return 1;
	return 0;
}


int bergantinUndido1(int posPart, int enemigo){
	int suma = 0;
	if(part.partidas[posPart].tablero[enemigo].bergantin1[0] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].bergantin1[1] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].bergantin1[2] == 1)
		suma++;
	if(suma == 3)
		return 1;
	return 0;
}
int bergantinUndido2(int posPart, int enemigo){
	int suma = 0;
	if(part.partidas[posPart].tablero[enemigo].bergantin2[0] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].bergantin2[1] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].bergantin2[2] == 1)
		suma++;
	if(suma == 3)
		return 1;
	return 0;
}
int destructorUndido(int posPart, int enemigo){
	int suma = 0;
	if(part.partidas[posPart].tablero[enemigo].destructor[0] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].destructor[1] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].destructor[2] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].destructor[3] == 1)
		suma++;

	if(suma == 4)
		return 1;
	return 0;
}
int portaUndido(int posPart, int enemigo){
	int suma = 0;
	if(part.partidas[posPart].tablero[enemigo].portaaviones[0] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].portaaviones[1] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].portaaviones[2] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].portaaviones[3] == 1)
		suma++;
	if(part.partidas[posPart].tablero[enemigo].portaaviones[4] == 1)
		suma++;

	if(suma == 5)
		return 1;
	return 0;
}



//Estructura necesaria para acceso excluyent

void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	//int socket_conn = * (int *) socket;
	
	char peticion[512];
	char respuesta[512];
	char enviar[512];
	int ret;

	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//inicializar la conexion
	if(server == 0)
		conn = mysql_real_connect (conn, "localhost","root", "mysql", "T4_BBDDJUEGO",0, NULL, 0);
	else if(server == 1)
		conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T4_BBDDJUEGO",0, NULL, 0);
	
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	int terminar =0;
	// Entramos en un bucle para atender todas las peticiones de este cliente
	//hasta que se desconecte
	while (terminar ==0)
	{
		sprintf(respuesta, "");
		// Ahora recibimos la peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		
		printf ("Peticion: %s\n",peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		// Ya tenemos el codigo de la petici?n
		
		char consulta [80];
		int posJug = posicionJugador(sock_conn);
		//guardamos la posicion del jugador en el vector de conectados (-1 si no esta conectado)
		int posEnPartida;
		int posPartida;
		int partJug;
		if(posJug != -1){//Verificamos si el jugador esta conectado
			partJug = conectados.usuarios[posJug].partida; //Esta variable = -1 si no esta en partida y = 1 si esta en partida
			if(partJug == 1){ //Verificamos si esta en partida
				posPartida = posicionPartida(sock_conn);
				for(int j = 0; j<2; j++)
					if (part.partidas[posPartida].socket[j] == sock_conn)
						posEnPartida = j;	
			}
		}
		
		if (codigo ==0){//petici?n de desconexi?n
			char usuario[20];
			strcpy(usuario,strtok(NULL, "/"));
			if(strcmp(usuario, "No iniciado")==0)
				sprintf(enviar, "3/Terminal desconectado!");
			else{
				EliminarConectado(usuario);
				sprintf(enviar, "3/Usuario %s desconectado!" ,usuario);
			}
			terminar=1;
		}
		else if (codigo == 1){ //inicio de sesion
			char usuario[20];
			strcpy(usuario,strtok(NULL, "/"));
			char password[20];
			strcpy(password,strtok( NULL, "/"));
			printf ("Usuario: %s, Contrasena: %s\n", usuario, password);
			
			char consulta [80];
			sprintf(consulta,"SELECT * FROM JUGADORES WHERE USUARIO = '%s' AND PASSWORD = '%s'" , usuario, password);
			
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL){
				printf ("No se han obtenido datos en la consulta\n");
				sprintf(enviar, "5/Usuario o Contrasena invalido!!");
			}
			else{
				AnadirConectado(usuario);
				sprintf(enviar, "1/ Login Completado!!");
			}
			write (sock_conn, enviar, strlen(enviar));

		}
		
		else if (codigo == 2){ 
			printf ("Registro\n");
			char nombre[20];
			strcpy(nombre,strtok( NULL, "/"));
			char usuario[20];
			strcpy(usuario,strtok( NULL, "/"));
			char password[20];
			strcpy(password,strtok( NULL, "/"));
			int edad  = atoi(strtok( NULL, "/"));
			char consulta [80];
			sprintf(consulta,"SELECT * FROM JUGADORES WHERE USUARIO = '%s'" , usuario);
			
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL){
				sprintf(consulta,"INSERT INTO JUGADORES(NOMBRE, USUARIO, PASSWORD, EDAD) VALUES('%s','%s','%s',%d);" , nombre, usuario, password, edad);
				err=mysql_query (conn, consulta);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				sprintf(enviar, "5/Cuenta creada!!");
			}
			else
				sprintf(enviar, "5/Usuario no disponible!!");

			write (sock_conn, enviar, strlen(enviar));
		}
		
		else if (codigo == 3){ 
			
			
			sprintf (consulta,"SELECT USUARIO FROM JUGADORES WHERE VICTORIAS = (SELECT MAX(VICTORIAS) FROM JUGADORES)");
			
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
				while (row !=NULL) {
					// la columna 0 contiene el nombre del jugador
					sprintf (respuesta, "%s %s",respuesta, row[0]);
					// obtenemos la siguiente fila
					row = mysql_fetch_row (resultado);
				}
			
			sprintf(enviar, "5/El usuario con mas victorias es %s",respuesta);
			write (sock_conn, enviar, strlen(enviar));
		}
		
		else if (codigo == 4){ 
			sprintf (consulta,"SELECT USUARIO FROM JUGADORES WHERE PUNTUACION = (SELECT MAX(PUNTUACION) FROM JUGADORES)");
			
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else{
				while (row !=NULL) {
					// la columna 0 contiene el nombre del jugador
					sprintf (respuesta, "%s %s",respuesta, row[0]);
					// obtenemos la siguiente fila
					row = mysql_fetch_row (resultado);
				}
			}
			sprintf(enviar, "1/El usuario con mas puntuacion es %s",respuesta);
			write(sock_conn, enviar, strlen(enviar));
			
		}
		
		else if (codigo == 5){ 
			char usuario[20];
			strcpy(usuario,strtok( NULL, ","));
			sprintf (consulta,"SELECT COUNT(*) AS PartidasJugadas FROM ENFRENTAMIENTOS AS E JOIN JUGADORES AS J ON E.ID_J1 = J.ID OR E.ID_J2 = J.ID WHERE J.USUARIO = '%s';" ,usuario);
			
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else{
				while (row !=NULL) {
					// la columna 0 contiene el nombre del jugador
					sprintf (respuesta, "%s", row[0]);
					// obtenemos la siguiente fila
					row = mysql_fetch_row (resultado);
				}
			}
			sprintf(enviar, "1/El usuario %s ha jugado %s partidas",usuario, respuesta);
			write(sock_conn, enviar, strlen(enviar));
		}
		else if(codigo == 6){ //creado de invitacion
			char usuario[20];
			strcpy(usuario ,strtok( NULL, ","));
			int sock_enemigo;
			for(int j=0;j<conectados.num; j++){
				if(strcmp(usuario, conectados.usuarios[j].usuario)==0)
					sock_enemigo = conectados.usuarios[j].socket;
			}
			char miUsuario[20];
			strcpy(miUsuario, conectados.usuarios[posJug].usuario);	
			sprintf(respuesta, "7/%s", conectados.usuarios[posJug].usuario);
			printf("\nEnvio: %s\n" ,respuesta);
			write(sock_enemigo, respuesta, strlen(respuesta));
		}
	
		else if(codigo == 7){ //Aceptacion de invitacion
			char usuario[20];
			strcpy(usuario,strtok( NULL, ","));
			int sock_enemigo;
			int estado;
			for(int j=0;j<conectados.num; j++){
				if(strcmp(usuario, conectados.usuarios[j].usuario)==0){
					sock_enemigo = conectados.usuarios[j].socket;
					if(conectados.usuarios[j].partida == 1)
						estado = -1;
				}
			}
			if(estado == -1){
				sprintf(respuesta, "5/%s ya esta en partida!", usuario);
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else{
				crearPartida(sock_enemigo, sock_conn);
				sprintf(respuesta, "11/", usuario);
				write(sock_conn, respuesta, strlen(respuesta));
				write(sock_enemigo, respuesta, strlen(respuesta));
			}
		}
		
		else if(codigo == 8){ //Mensajes por chat
			char mensaje[200];
			char usuario[20];
			strcpy(mensaje, strtok( NULL, "/"));
			for(int j =0; j<conectados.num; j++)
				if(sock_conn == conectados.usuarios[j].socket)
					strcpy(usuario, conectados.usuarios[j].usuario);
			sprintf(respuesta, "25/%s: %s" ,usuario, mensaje);
			printf("Envio: %s" , respuesta);
			write(part.partidas[posPartida].socket[1], respuesta, strlen(respuesta));
			write(part.partidas[posPartida].socket[0], respuesta, strlen(respuesta));
		}

		else if(codigo == 9){//guarda botes
			int v1 = atoi(strtok(NULL, ","));
			int v2 = atoi(strtok(NULL, ","));
			int v3 = atoi(strtok(NULL, ","));
			pthread_mutex_lock(&mutex);
				part.partidas[posPartida].tablero[posEnPartida].bote[0] = v1; //tablero alterable
				part.partidas[posPartida].tablero[posEnPartida].bote[1] = v2; //tablero alterable
				part.partidas[posPartida].tablero[posEnPartida].bote[2] = v3; //tablero alterable
				part.partidas[posPartida].tablero[posEnPartida+2].bote[0] = v1; //tablero de posiciones
				part.partidas[posPartida].tablero[posEnPartida+2].bote[1] = v2; //tablero de posiciones
				part.partidas[posPartida].tablero[posEnPartida+2].bote[2] = v3; //tablero de posiciones
			pthread_mutex_unlock(&mutex);
			char respuesta[200];
			sprintf(enviar, "12/");
			write (sock_conn, enviar, strlen(enviar));
		}

		else if(codigo == 10){//guarda bergantines
			int v1 = atoi(strtok(NULL, ","));
			int v2 = atoi(strtok(NULL, ","));
			int v3 = atoi(strtok(NULL, ","));
			if(part.partidas[posPartida].tablero[posEnPartida].posBergantin == 0 ){
				printf("bergantin1");
				pthread_mutex_lock(&mutex);
					part.partidas[posPartida].tablero[posEnPartida].bergantin1[0] = v1;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].bergantin1[1] = v2;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].bergantin1[2] = v3;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin1[0] = v1;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin1[1] = v2;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin1[2] = v3;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida].posBergantin ++;
				pthread_mutex_unlock(&mutex);
				sprintf(enviar, "13/");
				write (sock_conn, enviar, strlen(enviar));
			}
			else if(part.partidas[posPartida].tablero[posEnPartida].posBergantin == 1 ){
				pthread_mutex_lock(&mutex);
					part.partidas[posPartida].tablero[posEnPartida].bergantin2[0] = v1;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].bergantin2[1] = v2;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].bergantin2[2] = v3;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin2[0] = v1;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin2[1] = v2;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].bergantin2[2] = v3;//tablero de posiciones
				pthread_mutex_unlock(&mutex);
				sprintf(enviar, "14/");
				write (sock_conn, enviar, strlen(enviar));
			}
			
		}

		else if(codigo == 11){ //guarda destructores
			int v1 = atoi(strtok(NULL, ","));
			int v2 = atoi(strtok(NULL, ","));
			int v3 = atoi(strtok(NULL, ","));
			int v4 = atoi(strtok(NULL, ","));
				pthread_mutex_lock(&mutex);
					part.partidas[posPartida].tablero[posEnPartida].destructor[0] = v1;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].destructor[1] = v2;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].destructor[2] = v3;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].destructor[3] = v4;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida+2].destructor[0] = v1; //tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].destructor[1] = v2; //tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].destructor[2] = v3; //tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].destructor[3] = v4; //tablero de posiciones
				pthread_mutex_unlock(&mutex);
			char respuesta[200];
			sprintf(enviar, "15/");
			write (sock_conn, enviar, strlen(enviar));
		}
		else if(codigo == 12){//guarda portaaviones
			int v1 = atoi(strtok(NULL, ","));
			int v2 = atoi(strtok(NULL, ","));
			int v3 = atoi(strtok(NULL, ","));
			int v4 = atoi(strtok(NULL, ","));
			int v5 = atoi(strtok(NULL, ","));
				pthread_mutex_lock(&mutex);
					part.partidas[posPartida].tablero[posEnPartida].portaaviones[0] = v1;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].portaaviones[1] = v2;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].portaaviones[2] = v3;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].portaaviones[3] = v4;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida].portaaviones[4] = v5;//tablero alterable
					part.partidas[posPartida].tablero[posEnPartida+2].portaaviones[0] = v1;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].portaaviones[1] = v2;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].portaaviones[2] = v3;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].portaaviones[3] = v4;//tablero de posiciones
					part.partidas[posPartida].tablero[posEnPartida+2].portaaviones[4] = v5;//tablero de posiciones
					part.partidas[posPartida].estadoT[posEnPartida] = 5;
				pthread_mutex_unlock(&mutex);
			char respuesta[200];
			sprintf(enviar, "16/");
			write (sock_conn, enviar, strlen(enviar));
			if(part.partidas[posPartida].estadoT[0]== 5 && part.partidas[posPartida].estadoT[1]== 5){
				sprintf(enviar, "17/");
				write (part.partidas[posPartida].socket[0], enviar, strlen(enviar));
				sprintf(enviar, "18/");
				write (part.partidas[posPartida].socket[1], enviar, strlen(enviar));
			}
		}
		else if(codigo ==13){//ataque
			int atck = atoi(strtok(NULL, ","));
			int enemigo;
			if(posEnPartida == 0)
				enemigo = 1;
			else if(posEnPartida == 1)
				enemigo = 0;
			int algo = atacando(posPartida, enemigo, atck, sock_conn, posEnPartida);
			if(algo==0){//ha tocado agua
				sprintf(respuesta, "21/%d" ,atck); //no se que funcion del cliente que ha atacado seria que toco agua
				write(sock_conn, respuesta, strlen(respuesta));
				sprintf(respuesta, "22/%d" ,atck); //no se que funcion del cliente que ha recibido el ataque seria que le tocaron agua
				write(part.partidas[posPartida].socket[enemigo], respuesta, strlen(respuesta));
				printf("\nHe enviado: %s\n" ,respuesta);

			}
			else if(algo == 1){
/*				sprintf(respuesta, "17/"); *///cambio a espera
/*				write(sock_conn, respuesta, strlen(respuesta));*/
/*				sprintf(respuesta, "18/"); *///cambio ataque
/*				write(part.partidas[posPartida].socket[enemigo], respuesta, strlen(respuesta));*/
			}
			else if(algo == 2){
				printf("Final de partida");
				char usuarioEnemigo[20];
				int socket_enemigo = part.partidas[posPartida].socket[enemigo];
				char usuarioMio[20];
				int posEnemigo;
				strcpy(usuarioMio, conectados.usuarios[posJug].usuario);
				for(int j=0; j<conectados.num; j++){
					if(conectados.usuarios[j].socket == part.partidas[posPartida].socket[enemigo])
						strcpy(usuarioEnemigo, conectados.usuarios[j].usuario);
				}
				printf("\nUsuario enemigo: %s\n", usuarioEnemigo);
				printf("\nMi Usuario: %s\n", usuarioMio);
				sprintf(consulta,"UPDATE JUGADORES SET PUNTUACION=PUNTUACION+%d WHERE USUARIO='%s'" ,part.partidas[posPartida].puntuacion[enemigo], usuarioEnemigo);
				printf(consulta);
				err=mysql_query (conn, consulta);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}

				sprintf(consulta,"UPDATE JUGADORES SET PUNTUACION=PUNTUACION+%d WHERE USUARIO='%s'" ,part.partidas[posPartida].puntuacion[posEnPartida], usuarioMio);
				printf(consulta);
				err=mysql_query (conn, consulta);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				sprintf(consulta,"UPDATE JUGADORES SET VICTORIAS=VICTORIAS+1 WHERE USUARIO='%s'" , usuarioMio);
				printf(consulta);
				err=mysql_query (conn, consulta);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}

				pthread_mutex_lock(&mutex);
					conectados.usuarios[posJug].partida = -1;
					conectados.usuarios[posEnemigo].partida = -1;
					part.partidas[posPartida].socket[0] = 0;
					part.partidas[posPartida].socket[1] = 0;
				pthread_mutex_unlock(&mutex); 
				printf("\nEstado en partida jug1:%d\nEstado en partida Jug2:%d\nSocket 1:%d\nSocket 2:%d\n" , conectados.usuarios[posJug].partida, conectados.usuarios[posEnemigo].partida, part.partidas[posPartida].socket[0], part.partidas[posPartida].socket[1]);
				sprintf(enviar, "9/%s" ,usuarioMio);
				printf("\n%s\n" ,enviar);
				printf("\nSocket mio:%d\nSocket enemigo:%d\n", sock_conn, socket_enemigo);
				write(socket_enemigo, enviar, strlen(enviar));														   
				write(sock_conn, enviar, strlen(enviar));
			}
			 
		}
		else if(codigo==14){ //rendirse
			printf("Final de partida");
			int enemigo;
			if(posEnPartida == 0)
				enemigo = 1;
			else if(posEnPartida == 1)
				enemigo = 0;
			int socket_enemigo = part.partidas[posPartida].socket[enemigo];
			char usuarioEnemigo[20];
			char usuarioMio[20];
			int posEnemigo;
			strcpy(usuarioMio, conectados.usuarios[posJug].usuario);
			for(int j=0; j<conectados.num; j++){
				if(conectados.usuarios[j].socket == part.partidas[posPartida].socket[enemigo])
					strcpy(usuarioEnemigo, conectados.usuarios[j].usuario);
			}
			printf("\nUsuario enemigo: %s\n", usuarioEnemigo);
			printf("\nMi Usuario: %s\n", usuarioMio);
			sprintf(consulta,"UPDATE JUGADORES SET PUNTUACION=PUNTUACION+%d WHERE USUARIO='%s'" ,part.partidas[posPartida].puntuacion[enemigo], usuarioEnemigo);
			printf(consulta);
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			sprintf(consulta,"UPDATE JUGADORES SET PUNTUACION=PUNTUACION+%d WHERE USUARIO='%s'" ,part.partidas[posPartida].puntuacion[posEnPartida], usuarioMio);
			printf(consulta);
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			sprintf(consulta,"UPDATE JUGADORES SET VICTORIAS=VICTORIAS+1 WHERE USUARIO='%s'" , usuarioEnemigo);
			printf(consulta);
			err=mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n",
						mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			pthread_mutex_lock(&mutex);
			conectados.usuarios[posJug].partida = -1;
			conectados.usuarios[posEnemigo].partida = -1;
			part.partidas[posPartida].socket[0] = 0;
			part.partidas[posPartida].socket[1] = 0;
			pthread_mutex_unlock(&mutex); 
			printf("\nEstado en partida jug1:%d\nEstado en partida Jug2:%d\nSocket 1:%d\nSocket 2:%d\n" , conectados.usuarios[posJug].partida, conectados.usuarios[posEnemigo].partida, part.partidas[posPartida].socket[0], part.partidas[posPartida].socket[1]);
			sprintf(respuesta, "9/%s" ,usuarioEnemigo);
			printf("\n%s\n" ,respuesta);
			printf("Socket enemigo: %s" ,part.partidas[posPartida].socket[enemigo]);
			write(socket_enemigo, respuesta, strlen(respuesta));														   
			write(sock_conn, respuesta, strlen(respuesta));
		}		
	}
		// Se acabo el servicio para este cliente
	close(sock_conn); 
}


int main(int argc, char *argv[])
{

	conectados.num = 0;
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// establecemos el puerto de escucha
	
	if(server == 0)
		serv_adr.sin_port = htons(pLcl);	
	else if(server == 1)
		serv_adr.sin_port = htons(pSvr);
	
	
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
	
	contador =0;
	
	pthread_t thread[100];
	i=0;
	// Bucle para atender a 5 clientes
	for (;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		
		sockets[i] = sock_conn;
		//sock_conn es el socket que usaremos para este cliente
		
		// Crear thead y decirle lo que tiene que hacer
		
		pthread_create (&thread[i], NULL, AtenderCliente,&sockets[i]);
	}
	
	
	
}
