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

typedef struct{
	char usuario[20];
	int socket;
} Tusuario;

typedef struct{
	Tusuario usuarios[512];
	int num;
} Tconectados;
Tconectados conectados;
int contador;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int i;
int sockets[100];

/*char listaUsuarios(){
	char lista[200];
	sprintf(lista, "%s", conectados.usuarios[0].usuario);
	if(conectados.num>1){
		for(int w=1; w<conectados.num; w++)
			sprintf(lista, "%s/%s" ,lista, conectados.usuarios[w].usuario);
	}
	return lista;
}*/
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
	conectados.num++;
	pthread_mutex_unlock( &mutex);
	printf("Nuevo usuario conectado %d: %s\n" , conectados.num, conectados.usuarios[conectados.num-1].usuario);
	
	sprintf(lista, "%s", conectados.usuarios[0].usuario);
	if(conectados.num>1){
		for(int w=1; w<conectados.num; w++)
			sprintf(lista, "%s,%s" ,lista, conectados.usuarios[w].usuario);
	}
	printf("Lista= %s\n", lista);
	sprintf(enviar,"4/%s",lista);
	printf("%s\n" ,enviar);
	broadcast(enviar);
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
	//inicializar la conexin
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "JUEGO",0, NULL, 0);
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
		// Ahora recibimos la petici?n
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		
		printf ("Peticion: %s\n",peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		// Ya tenemos el c?digo de la petici?n
		
		char consulta [80];
		
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
		else if (codigo == 1){ 
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
				sprintf(enviar, "1/Usuario o Contrasena invalido!!");
			}
			else{
				AnadirConectado(usuario);
				sprintf(enviar, "2/ Login Completado!!");
			}

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
				sprintf(enviar, "1/Cuenta creada!!");
			}
			else
				sprintf(enviar, "1/Usuario no disponible!!");
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
			
			sprintf(enviar, "1/El usuario con mas victorias es %s",respuesta);
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
			else
				while (row !=NULL) {
					// la columna 0 contiene el nombre del jugador
					sprintf (respuesta, "%s %s",respuesta, row[0]);
					// obtenemos la siguiente fila
					row = mysql_fetch_row (resultado);
			}
				
				sprintf(enviar, "1/El usuario con mas puntuacion es %s",respuesta);
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
			else
				while (row !=NULL) {
					// la columna 0 contiene el nombre del jugador
					sprintf (respuesta, "%s", row[0]);
					// obtenemos la siguiente fila
					row = mysql_fetch_row (resultado);
			}
				
				sprintf(enviar, "1/El usuario %s ha jugado %s partidas",usuario, respuesta);
		}
		
		printf ("Enviamos: %s\n", enviar);
		// Enviamos respuesta
		write (sock_conn, enviar, strlen(enviar));
				
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
	serv_adr.sin_port = htons(50075);
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
