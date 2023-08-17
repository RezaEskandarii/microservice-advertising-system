package main

import (
	_ "github.com/lib/pq"
)

func main() {
	app := NewApp()
	app.Run(5002)
}
