package main

import (
	"fmt"
	"os"
	"os/exec"
)

func main() {
	// Define the command and its arguments
	cmd := exec.Command("tesseract", "-l", "eng", "./9130082436.jpg", "stdout")

	// Set the command's standard output to os.Stdout
	cmd.Stdout = os.Stdout

	// Run the command
	err := cmd.Run()
	if err != nil {
		fmt.Println("========================================================")
		fmt.Println("Error:", err)
		fmt.Println("========================================================")
	}
}
