cmake_minimum_required(VERSION 3.16)

project(flecs C)
set(CMAKE_C_STANDARD 11)

get_filename_component(FLECS_DIRECTORY_PATH "../../../../ext/flecs" REALPATH BASE_DIR "${CMAKE_CURRENT_SOURCE_DIR}")
file(GLOB_RECURSE SRC ${CMAKE_CURRENT_SOURCE_DIR}/src/*.c;${FLECS_DIRECTORY_PATH}/src/*.c)

include_directories(flecs PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/include ${FLECS_DIRECTORY_PATH}/include)

add_library(flecs SHARED ${SRC})

if (WIN32)
    target_link_libraries(flecs wsock32 ws2_32)
endif()