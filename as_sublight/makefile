CXX:=g++

ifeq ($(DEBUG), yes)
    CXXFLAGS:= -c -O0 -Wall
	OUT:=Debug
else
    CXXFLAGS:= -c -O3 -Wall
	OUT:=Release
endif

SRC_DIR:=source
BIN_DIR:=./../bin/$(OUT)
OBJ_DIR:=obj
OUTPUT := $(BIN_DIR)/as_sublight.dll

CPP_FILES := \
	$(SRC_DIR)/sublight.cpp \
	$(SRC_DIR)/sublightUDP.cpp \
	$(SRC_DIR)/main.cpp
	
LIBS := "c:\Program Files\Microsoft SDKs\Windows\v6.0A\Lib\Ws2_32.lib"

OBJS_FILES:=${CPP_FILES:$(SRC_DIR)/%.cpp=$(OBJ_DIR)/%.o} 

$(OBJ_DIR)/%.o: $(SRC_DIR)/%.cpp
	$(CXX) $(CXXFLAGS) $< -o $@
    
as_sublight: build_dirs $(OBJS_FILES)
	$(CXX) $(OBJS_FILES) -s -shared -Wl,--subsystem,windows $(LIBS) -o $(OUTPUT)

all: clean build_dirs as_sublight

build_dirs:
	mkdir -p $(OBJ_DIR)
	mkdir -p $(BIN_DIR)
    
clean:
	rm -rf $(OBJ_DIR)  
	rm -f $(OUTPUT)

.PHONY: clean
