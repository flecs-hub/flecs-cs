cmake_minimum_required(VERSION 3.16)

project(flecs C)
set(CMAKE_C_STANDARD 11)

get_filename_component(FLECS_DIRECTORY_PATH "../../../../ext/flecs" REALPATH BASE_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
include(${FLECS_DIRECTORY_PATH}/cmake/target_default_compile_warnings.cmake)
include(${FLECS_DIRECTORY_PATH}/cmake/target_default_compile_options.cmake)
file(GLOB_RECURSE INC ${CMAKE_CURRENT_SOURCE_DIR}/include/*.h ${FLECS_DIRECTORY_PATH}/include/*.h)
file(GLOB_RECURSE SRC ${CMAKE_CURRENT_SOURCE_DIR}/src/*.c ${FLECS_DIRECTORY_PATH}/src/*.c)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include ${FLECS_DIRECTORY_PATH}/include)

message("FILES: ${INC} ${SRC}")

add_library(flecs SHARED ${INC} ${SRC})

set_target_properties(flecs PROPERTIES LINKER_LANGUAGE C)

if(WIN32)
    target_link_libraries(flecs wsock32 ws2_32)
endif()

#target_default_compile_options_c(flecs)
#target_default_compile_warnings_c(flecs)
#target_include_directories(flecs PUBLIC ${FLECS_DIRECTORY_PATH}/include ${CMAKE_CURRENT_SOURCE_DIR}/include)