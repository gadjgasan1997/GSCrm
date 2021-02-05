class ProductCategory {
    Create(event) {
        return new Promise((resolve, reject) => {
            
        })
    }

    CreateGetData() {
        return {

        }
    }

    Update(event) {
        return new Promise((resolve, reject) => {
            
        })
    }

    UpdateGetData() {
        return {
            
        }
    }

    Delete(event) {
        return new Promise((resolve, reject) => {
            Swal.fire(MessageManager.Invoke("RemoveCategoryConfirmation")).then(dialogResult => {
                if (dialogResult.value) {
                    
                }
            })
        })
    }
}