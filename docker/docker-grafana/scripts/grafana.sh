# Grafana administration script to modify plugins and reset admin password.

#------------------------------------------- CLI ARGUMENTS -------------------------------------------------------------
option=$1
pluginArg=$2
pluginIdArg=$3
newAdminPassword=$2
#-----------------------------------------------------------------------------------------------------------------------

function grafana_server_invalid_option() {
	echo "Invalid option! Execute \"./grafana.sh help\" for more information"
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER LIST AVAILABLE PLUGINS ----------------------------------------
function grafana_server_list_plugins() {
	grafana-cli plugins list-remote | less
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER LIST INSTALLED PLUGINS ----------------------------------------
function grafana_server_list_installed_plugins() {
	grafana-cli plugins ls | less 
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER INSTALL PLUGIN WITH ID ----------------------------------------
function grafana_server_install_plugin() {
	grafana-cli plugins install $pluginIdArg
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER REMOVE PLUGIN WITH ID ------------------------------------------
function grafana_server_remove_plugin() {
	echo $(grafana-cli plugins remove $pluginIdArg | grep "Plugin does not exist")
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER UPDATE PLUGIN WITH ID ------------------------------------------
function grafana_server_update_plugin() {
	echo $(grafana-cli plugins update $plugIdArg | grep "Could not find")
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER UPDATE ALL PLUGINS ---------------------------------------------
function grafana_server_update_all_plugins() {
	echo $(grafana-cli plugins update-all | grep "OK")
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER SEARCH PLUGINS -------------------------------------------------
function grafana_server_search_plugins() {
	echo "############################# AVAILABLE PLUGINS ###########################"
	availablePlugins=$(grafana-cli plugins list-remote | grep $pluginIdArg)
	if [[ $availablePlugins == "" ]]
		then 
			printf "There is not exist plugin with id \"$pluginIdArg\"\n"  
	else 
		printf "$availablePlugins\n\n"
	fi

	printf "###########################################################################\n\n"

	echo "############################ INSTALLED PLUGINS ############################"
	installedPlugins=$(grafana-cli plugins ls | grep $pluginIdArg)
	if [[ $installedPlugins == "" ]]
		then 
			printf "There is not exist plugin with id \"$pluginIdArg\"\n"
	else 
		printf "$installedPlugins\n\n"
	fi
	printf "###########################################################################\n\n"
}
#-----------------------------------------------------------------------------------------------------------------------

#--------------------------------------- GRAFANA SERVER RESET ADMIN PASS -----------------------------------------------
function grafana_server_reset_admin_password() {
	grafana-cli admin reset-admin-password --homepath /usr/share/grafana $newAdminPassword
	echo "Admin password reset completed..."
}
#-----------------------------------------------------------------------------------------------------------------------


#--------------------------------------- GRAFANA SERVER HELP DOCUMENTATION ---------------------------------------------
function grafana_server_help() {
	less << End
#################################################################################################
#			       	GRAFANA SERVER - SCRIPT TOOL HELP                               #
#################################################################################################
-------------- OPTIONS ---------------
# - plugin (list plugins (available and installed), search, install, remove and update plugins)
# - reset-admin-password
# - help

#-----------------------------------------------------------------------------------------------------------------------
# Reset admin passwords: 
#	./grafana.sh reset-admin-password newAdminPassword
#-----------------------------------------------------------------------------------------------------------------------

#-----------------------------------------------------------------------------------------------------------------------
# NOTE: Listed in less program in terminal. To quit press q. Up and arrow keys to scroll also.
#  Listing available plugins:
# 	./grafana.sh plugin list-all
#
#  Listing installed plugins:
#	./grafana.sh plugin list-install
#  
#  Searching plugins:
# 	./grafana.sh plugin search pluginId
#
#	Installing plugins
#	 ./grafana.sh plugin install pluginId
#	
#	Removing plugins:
#	 ./grafana.sh plugin remove pluginId
#
#	Updating plugins:
#	 ./grafana.sh plugin update pluginId
#	 ./grafana.sh plugin update-all
#-----------------------------------------------------------------------------------------------------------------------
End
}	
#-----------------------------------------------------------------------------------------------------------------------

#----------------------------------------- GRAFANA SCRIPT MENU ---------------------------------------------------------
case $option in 
	"help")
		grafana_server_help;;
	"reset-admin-password")
		grafana_server_reset_admin_password;;
	"plugin")
		case $pluginArg in 
			"list-all" )
				grafana_server_list_plugins;;
			"list-installed")
				grafana_server_list_installed_plugins;;
			"update")
				grafana_server_update_plugin;;
			"update-all")
				grafana_server_update_all_plugins;;
			"install")
				grafana_server_install_plugin;;
			"remove")
				grafana_server_remove_plugin;;
			"search")
				grafana_server_search_plugins;;
			*)
				grafana_server_invalid_option
		esac;;
	*) 
		grafana_server_invalid_option
esac 
#-----------------------------------------------------------------------------------------------------------------------