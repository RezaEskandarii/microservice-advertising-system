package main

import (
	env "category-management/pkg/env_manager"
	"strconv"
)

func main() {
	app := NewApp()
	portStr := env.GetFromDotENV("app_port")

	if port, err := strconv.Atoi(portStr); err != nil {

		panic(err.Error())

	} else {

		app.Run(port)
	}

}
