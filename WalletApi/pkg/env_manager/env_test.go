package env_manager

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

func TestEnvPackage(t *testing.T) {
	var key = "test_env"
	var value = "1.20_test"

	t.Run("Test_Can_Set_Env", func(t *testing.T) {
		err := Set(key, value)
		assert.Nil(t, err)
	})

	t.Run("Test_Can_Read_Env", func(t *testing.T) {
		env := LoadEnv(key)
		assert.NotNil(t, env)
		assert.Equal(t, value, env)
	})
}
